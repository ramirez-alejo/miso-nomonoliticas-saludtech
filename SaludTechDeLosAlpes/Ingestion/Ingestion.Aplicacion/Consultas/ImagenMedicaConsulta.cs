using Core.Dominio;
using Ingestion.Infraestructura.Persistencia.Repositorios;
using Mediator;

namespace Ingestion.Aplicacion.Consultas;

public class ImagenMedicaConsulta : IRequest<ImagenMedicaResponse>
{
	public Guid[] Ids { get; set; }
	public Demografia Demografia { get; set; }
	public Modalidad Modalidad { get; set; }
}

public class ImagenMedicaResponse
{
	public Imagen[] Imagenes { get; set; }
}

public class ImagenMedicaConsultaHandler(IImagenRepository imagenRepository, ILogger<ImagenMedicaConsultaHandler> logger) : IRequestHandler<ImagenMedicaConsulta, ImagenMedicaResponse>
{
	public async ValueTask<ImagenMedicaResponse> Handle(ImagenMedicaConsulta request, CancellationToken cancellationToken)
	{
		if (request.Ids?.Any() == true)
		{
			var result = await Task.WhenAll(request.Ids.Select(id => imagenRepository.GetByIdAsync(id, cancellationToken)));
			logger.Log(LogLevel.Information, "Imagenes medicas consultadas por ids");
			return new ImagenMedicaResponse { Imagenes = result };
		}
		if (request.Demografia != null || request.Modalidad != null)
		{
			var result = await imagenRepository.GetByModalidadAndDemografiaAsync(request.Modalidad, request.Demografia, cancellationToken);
			logger.Log(LogLevel.Information, "Imagenes medicas consultadas por filtro");
			return new ImagenMedicaResponse { Imagenes = result.ToArray() };
		}
		logger.Log(LogLevel.Information, "Imagenes medicas consultadas sin filtro");
		return new ImagenMedicaResponse { Imagenes = [] };
	}
}