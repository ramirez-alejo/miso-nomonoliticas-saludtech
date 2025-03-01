using Consulta.Aplicacion.Comandos;
using Consulta.Aplicacion.Consultas;
using Consulta.Aplicacion.Sagas;
using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Workers;
using Core.Infraestructura;
using Mediator;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all 
#if RELEASE
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});
#endif

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

// Register saga services
builder.Services.AddSingleton<ISagaStateRepository, InMemorySagaStateRepository>();
builder.Services.AddScoped<ImagenConsultaSagaOrchestrator>();

// Register background workers
builder.Services.AddHostedService<ImagenSubscriptionWorker>();
builder.Services.AddHostedService<ImagenConsultaDataWorker>();
builder.Services.AddHostedService<ImagenConsultaResponseWorker>();

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

// Saga-based API endpoints
app.MapPost("/api/consultas/imagen", async (IMediator mediator, ImagenMedicaConsultaConFiltros command) =>
{
    var sagaId = await mediator.Send(new StartImagenConsultaSagaCommand(command));
    return Results.Accepted($"/api/consultas/imagen/status/{sagaId}", new { sagaId = sagaId.ToString() });
});

app.MapGet("/api/consultas/imagen/status/{sagaId}", async (Guid sagaId, IMediator mediator) =>
{
    var state = await mediator.Send(new GetImagenConsultaSagaStateQuery(sagaId));
    if (state == null)
        return Results.NotFound();
        
    if (state.Status == "Completed")
        return Results.Ok(new ImagenMedicaResponse { Imagenes = state.FinalResults });
        
    if (state.Status == "Failed")
        return Results.BadRequest(new { error = state.ErrorMessage });
        
    return Results.Accepted($"/api/consultas/imagen/status/{sagaId}", new { status = state.Status });
});


app.Run();
