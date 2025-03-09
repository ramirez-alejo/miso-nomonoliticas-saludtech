using BFF.API.Persistence;
using BFF.API.Services;
using BFF.API.Workers;
using Core.Infraestructura;
using Core.Infraestructura.MessageBroker;

namespace BFF.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure Kestrel to listen on all interfaces
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(80);
        });
        
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

        // Log to the console
        builder.Logging.AddConsole();

        // Register MessageBroker services
        builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
        builder.Services.AddSingleton<IMessageConsumer, MessageConsumer>();
        
        // Configure Redis connection
        var redisConnection = Environment.GetEnvironmentVariable("Redis:ConnectionString") 
                             ?? builder.Configuration.GetValue<string>("Redis:ConnectionString") 
                             ?? "redis:6379";
                             
        // Register correlation repository
        builder.Services.AddSingleton<ICorrelationRepository>(provider => 
            new RedisCorrelationRepository(
                redisConnection, 
                provider.GetRequiredService<ILogger<RedisCorrelationRepository>>()));
        
        // Register application services
        builder.Services.AddScoped<IImagenService, ImagenService>();
        
        // Register background workers
        builder.Services.AddHostedService<SagaIniciadaSubscriptionWorker>();
        
        builder.Services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "SaludTechAPI-BFF";
            config.Title = "SaludTechAPI-BFF v1";
            config.Version = "v1";
        });
        
        var app = builder.Build();
        app.UseCustomExceptionHandler();
        app.UseOpenApi();
        app.UseSwaggerUi(config =>
        {
            config.DocumentTitle = "SaludTechAPI-BFF";
            config.Path = "/swagger";
            config.DocumentPath = "/swagger/{documentName}/swagger.json";
            config.DocExpansion = "list";
            config.PersistAuthorization = true;
        });
        
        app.MapGet("/health", () => Results.Ok());
        app.MapGet("/version", () => Results.Ok("1.0.0"));
        
        app.MapControllers();
        
        app.Run();
    }
}
