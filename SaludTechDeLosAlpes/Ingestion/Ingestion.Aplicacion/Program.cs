using Core.Infraestructura;
using Ingestion.Aplicacion.Comandos;
using Ingestion.Aplicacion.Consultas;
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
		
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING:DefaultConnection")
			?? builder.Configuration.GetConnectionString("DefaultConnection");
		
		// Log to the console
		builder.Logging.AddConsole();
		
		builder.Services.AddDbContext<ImagenDbContext>(options =>
			options.UseNpgsql(connectionString, o =>
				{
					o.EnableRetryOnFailure(5);
				})
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
		
		builder.Services.AddScoped<IImagenRepository, ImagenRepository>();
		
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
		
		app.MapPost("/api/imagenes/buscar", async (ImagenMedicaConsulta consulta, IMediator mediator) =>
		{
			var response = await mediator.Send(consulta);
			return response is null ? Results.NotFound() : Results.Ok(response);
		});
		
		app.Run();
	}
}