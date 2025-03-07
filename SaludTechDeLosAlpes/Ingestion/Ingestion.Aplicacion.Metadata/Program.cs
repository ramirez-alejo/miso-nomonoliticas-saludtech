using Core.Infraestructura;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Metadata.Handlers;
using Ingestion.Aplicacion.Metadata.Persistencia;
using Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;
using Ingestion.Aplicacion.Metadata.Workers;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Aplicacion.Metadata;

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

		// Configure database
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection")
		    ?? builder.Configuration.GetConnectionString("DefaultConnection")
		    ?? "Host=localhost;Database=SaludTechMetadataDb;Username=postgres;Password=postgres";

		builder.Services.AddDbContext<MetadataDbContext>(options =>
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
		builder.Services.AddScoped<IMetadataRepository, MetadataRepository>();
		builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
		builder.Services.AddScoped<IMessageConsumer, MessageConsumer>();
		builder.Services.AddScoped<GenerarMetadataHandler>();
		builder.Services.AddScoped<EliminarMetadataHandler>();
		builder.Services.AddHostedService<MetadataImagenSubscriptionWorker>();
		builder.Services.AddHostedService<EliminarMetadataSubscriptionWorker>();

		// Add mediator
		builder.Services.AddMediator(options =>
		    options.ServiceLifetime = ServiceLifetime.Scoped);

		// Configure Swagger/OpenAPI
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddOpenApiDocument(config =>
		{
		    config.DocumentName = "SaludTechAPI-Metadata";
		    config.Title = "SaludTechAPI-Metadata API";
		    config.Version = "v1";
		});

		var app = builder.Build();

		app.UseCustomExceptionHandler();
		app.UseOpenApi();
		app.UseSwaggerUi(config =>
		{
		    config.DocumentTitle = "SaludTechAPI-Metadata";
		    config.Path = "/swagger";
		    config.DocumentPath = "/swagger/{documentName}/swagger.json";
		    config.DocExpansion = "list";
		    config.PersistAuthorization = true;
		});

		// Ensure database is created and migrations are applied
		using (var scope = app.Services.CreateScope())
		{
		    var context = scope.ServiceProvider.GetRequiredService<MetadataDbContext>();
		    context.Database.Migrate();
		}

		app.MapGet("/health", () => Results.Ok());
		app.MapGet("/version", () => Results.Ok("1.0.0"));
		app.Run();
	}
}
