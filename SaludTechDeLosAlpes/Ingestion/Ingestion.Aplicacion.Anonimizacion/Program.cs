using Core.Infraestructura;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Anonimizacion.Comandos;
using Ingestion.Aplicacion.Anonimizacion.Persistencia;
using Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios;
using Ingestion.Aplicacion.Anonimizacion.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all interfaces
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

// Configure database
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=SaludTechAnonimizarDb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AnonimizarDbContext>(options =>
    options.UseNpgsql(connectionString, o =>
        {
            o.EnableRetryOnFailure(5);
        })
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// Configure message broker
var brokerHost = Environment.GetEnvironmentVariable("MessageBroker:Host")
    ?? builder.Configuration.GetValue<string>("MessageBroker:Host")
    ?? "localhost";

var brokerPort = Environment.GetEnvironmentVariable("MessageBroker:Port") != null
    ? int.Parse(Environment.GetEnvironmentVariable("MessageBroker:Port"))
    : builder.Configuration.GetValue<int>("MessageBroker:Port", 6650);

// Configure MessageBroker settings
builder.Services.Configure<MessageBrokerSettings>(settings =>
{
    settings.Host = brokerHost;
    settings.Port = brokerPort;
});
builder.Services.AddSingleton<MessageBrokerSettings>();

// Log to the console
builder.Logging.AddConsole();

// Register repositories and services
builder.Services.AddScoped<IAnonimizarRepository, AnonimizarRepository>();
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddSingleton<IMessageConsumer, MessageConsumer>();
builder.Services.AddScoped<AnonimizarHandler>();
builder.Services.AddScoped<EliminarAnonimizacionHandler>();
builder.Services.AddHostedService<AnonimizarImagenSubscriptionWorker>();
builder.Services.AddHostedService<EliminarAnonimizacionSubscriptionWorker>();

// Add mediator
builder.Services.AddMediator(options =>
    options.ServiceLifetime = ServiceLifetime.Scoped);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "SaludTechAPI-Anonimizacion";
    config.Title = "SaludTechAPI-Anonimizacion API";
    config.Version = "v1";
});

var app = builder.Build();

app.UseCustomExceptionHandler();
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "SaludTechAPI-Anonimizacion";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
    config.PersistAuthorization = true;
});

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AnonimizarDbContext>();
    context.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok());
app.MapGet("/version", () => Results.Ok("1.0.0"));
app.Run();
