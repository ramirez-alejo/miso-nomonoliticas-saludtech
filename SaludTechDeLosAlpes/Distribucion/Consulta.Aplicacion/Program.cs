using Consulta.Aplicacion.Comandos;
using Consulta.Aplicacion.Consultas;
using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Workers;
using Core.Infraestructura;
using Mediator;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all interfaces
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

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
builder.Services.AddHttpClient();
builder.Services.AddMediator(options =>
    options.ServiceLifetime = ServiceLifetime.Scoped);

// Register message broker services
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddScoped<IMessageConsumer, MessageConsumer>();
builder.Services.AddScoped<ImagenCreadaHandler>();

// Register background worker
builder.Services.AddHostedService<ImagenSubscriptionWorker>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "SaludTechAPI-Consultas";
    config.Title = "SaludTechAPI-Consultas v1";
    config.Version = "v1";
});

var app = builder.Build();

app.UseCustomExceptionHandler();
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "SaludTechAPI-Consultas";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
    config.PersistAuthorization = true;
});

app.MapGet("/health", () => Results.Ok());
app.MapGet("/version", () => Results.Ok("1.0.0"));
app.MapPost("/api/consultas/imagen", async (IMediator mediator, ImagenMedicaConsultaConFiltros command) =>
{
    await mediator.Send(command);
    return Results.Accepted();
});


app.Run();
