using Core.Infraestructura;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Comandos;
using Ingestion.Aplicacion.Consultas;
using Ingestion.Aplicacion.Dtos;
using Ingestion.Infraestructura.Persistencia;
using Ingestion.Infraestructura.Persistencia.Repositorios;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Aplicacion;

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
		
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection")
							   ?? builder.Configuration.GetConnectionString("DefaultConnection");

		
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
		
		// Register HttpClient
		builder.Services.AddHttpClient();
		
// Register background workers
builder.Services.AddHostedService<Workers.ImagenConsultaDataWarehouseRequestWorker>();
builder.Services.AddHostedService<Workers.AnonimizadaSubscriber>();
builder.Services.AddHostedService<Workers.MetadataGeneradaSubscriber>();
builder.Services.AddHostedService<Workers.SolicitudProcesamientoImagenWorker>();
		
		builder.Services.AddDbContext<ImagenDbContext>(options =>
			options.UseNpgsql(connectionString, o =>
				{
					o.EnableRetryOnFailure(5);
				})
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
		
		builder.Services.AddScoped<IImagenRepository, ImagenRepository>();
		
		// Configure Redis connection
		var redisConnection = Environment.GetEnvironmentVariable("Redis:ConnectionString") 
							 ?? builder.Configuration.GetValue<string>("Redis:ConnectionString") 
							 ?? "redis:6379";
							 
		// Register saga services
		builder.Services.AddSingleton<Ingestion.Aplicacion.Sagas.ISagaStateRepository>(provider => 
			new Ingestion.Aplicacion.Sagas.RedisSagaStateRepository(redisConnection));
		builder.Services.AddScoped<Ingestion.Aplicacion.Sagas.ImagenIngestionSagaOrchestrator>();
		
		builder.Services.AddMediator(options =>
			options.ServiceLifetime = ServiceLifetime.Scoped);

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddOpenApiDocument(config =>
		{
			config.DocumentName = "SaludTechAPI-Ingestion";
			config.Title = "SaludTechAPI-Ingestion v1";
			config.Version = "v1";
		});
		
		
		var app = builder.Build();
		app.UseCustomExceptionHandler();
		app.UseOpenApi();
		app.UseSwaggerUi(config =>
		{
			config.DocumentTitle = "SaludTechAPI-Ingestion";
			config.Path = "/swagger";
			config.DocumentPath = "/swagger/{documentName}/swagger.json";
			config.DocExpansion = "list";
			config.PersistAuthorization = true;
		});
		
		// If the Db doesn't exist, create it
		using var scope = app.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<ImagenDbContext>();

		// Apply the migrations
		dbContext.Database.Migrate();

		app.MapGet("/health", () => Results.Ok());
				
		app.MapGet("/version", () => Results.Ok("1.0.0"));
				
		app.MapPost("/api/imagenes", async (CrearImagenMedicaCommand command, IMediator mediator) =>
		{
			var response = await mediator.Send(command);
			return Results.Created($"/api/imagenes/{response.Id}", response);
		});

		app.MapPost("/api/imagenes/procesar", async (CrearImagenMedicaCommand imagen, IMediator mediator) =>
		{
			var result = await mediator.Send(imagen);
			var command = new StartImagenIngestionSagaCommand(
				new ImagenDto
				{
					Id = result.Id,
					Descripcion = $"Modalidad: {imagen.TipoImagen?.Modalidad?.Nombre}, Región: {imagen.TipoImagen?.RegionAnatomica?.Nombre}",
					FechaCreacion = DateTime.Now,
					Imagen = imagen
				});
			var sagaId = await mediator.Send(command);
			return Results.Accepted($"/api/imagenes/procesar/{sagaId}", new { SagaId = sagaId });
		});

		app.MapGet("/api/imagenes/procesar/{sagaId}", async (Guid sagaId, IMediator mediator) =>
		{
			var query = new Ingestion.Aplicacion.Consultas.GetImagenIngestionSagaStateQuery(sagaId);
			var state = await mediator.Send(query);
			return state is null ? Results.NotFound() : Results.Ok(state);
		});
		
		app.MapPost("/api/imagenes/ids", async (ImagenMedicaConsulta consulta, IMediator mediator) =>
		{
			var response = await mediator.Send(consulta);
			return response is null ? Results.NotFound() : Results.Ok(response);
		});
		
		app.MapPost("/api/imagenes/buscar", async (ImagenMedicaConsulta consulta, IMediator mediator) =>
		{
			var response = await mediator.Send(consulta);
			return response is null ? Results.NotFound() : Results.Ok(response);
		});
		
		app.Run();
	}
}
