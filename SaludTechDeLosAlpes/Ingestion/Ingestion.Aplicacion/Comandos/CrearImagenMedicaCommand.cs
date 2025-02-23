using Core.Dominio;
using Ingestion.Infraestructura.Persistencia.Repositorios;
using Mediator;

namespace Ingestion.Aplicacion.Comandos;

public class CrearImagenMedicaCommand : Imagen, IRequest<ImagenMedicaResponse>
{
	
}

public class ImagenMedicaResponse
{
	public Guid Id { get; set; }
}

public class CrearImagenMedicaHandler(IImagenRepository imagenRepository, ILogger<CrearImagenMedicaHandler> logger) : IRequestHandler<CrearImagenMedicaCommand, ImagenMedicaResponse>
{
	public async ValueTask<ImagenMedicaResponse> Handle(CrearImagenMedicaCommand command, CancellationToken cancellationToken)
	{
		var result = await imagenRepository.AddAsync(command, cancellationToken);
		logger.Log(LogLevel.Information, "Imagen medica creada con id {ResultId}", result.Id);
		return new ImagenMedicaResponse { Id = result.Id };
	}
}