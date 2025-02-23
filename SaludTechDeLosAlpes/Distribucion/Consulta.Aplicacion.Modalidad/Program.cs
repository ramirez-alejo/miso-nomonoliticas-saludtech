using Consulta.Aplicacion.Modalidad.Comandos;
using Core.Infraestructura;
using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Modalidad.Persistencia;
using Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;
using Consulta.Aplicacion.Modalidad.Workers;
using Consulta.Aplicacion.Modalidad.Consultas;
using Mediator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure database
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=SaludTechModalidadDb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ImagenModalidadDbContext>(options =>
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
builder.Services.Configure<MessageBrokerSettings>(settings => {
    settings.Host = brokerHost;
    settings.Port = brokerPort;
});
builder.Services.AddSingleton<MessageBrokerSettings>();

// Log to the console
builder.Logging.AddConsole();

// Register repositories and services
builder.Services.AddScoped<IImagenModalidadRepository, ImagenModalidadRepository>();
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddScoped<IMessageConsumer, MessageConsumer>();
builder.Services.AddScoped<ImagenModalidadHandler>();
builder.Services.AddHostedService<ImagenModalidadSubscriptionWorker>();

// Add mediator
builder.Services.AddMediator(options =>
    options.ServiceLifetime = ServiceLifetime.Scoped);

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "SaludTechAPI-Modalidad";
    config.Title = "SaludTechAPI-Modalidad v1";
    config.Version = "v1";
});

var app = builder.Build();

app.UseCustomExceptionHandler();
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "SaludTechAPI-Modalidad";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
    config.PersistAuthorization = true;
});

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ImagenModalidadDbContext>();
    context.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok());
app.MapGet("/version", () => Results.Ok("1.0.0"));

// API endpoints
app.MapPost("/api/modalidad/buscar", async (IMediator mediator, ImagenModalidadConsulta query) =>
{
    var result = await mediator.Send(query);
    return Results.Ok(result);
});


app.Run();
