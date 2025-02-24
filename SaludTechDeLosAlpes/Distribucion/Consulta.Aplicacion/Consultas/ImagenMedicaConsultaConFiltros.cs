using Consulta.Aplicacion.Dtos;
using Consulta.Aplicacion.Mapeo;
using Consulta.Dominio;
using Core.Dominio;
using Mediator;

namespace Consulta.Aplicacion.Consultas;

public class ImagenMedicaConsultaConFiltros : IRequest<ImagenMedicaResponse>
{
	public Guid[] Ids { get; set; }
	public ImagenDemografia Demografia { get; set; }
	public ImagenModalidad Modalidad { get; set; }
}

public class ImagenMedicaResponse
{
	public ImagenMedica[] Imagenes { get; set; }
}

public class ImagenMedicaConsultaConFiltrosHandler(HttpClient httpClient, IConfiguration configuration) : IRequestHandler<ImagenMedicaConsultaConFiltros, ImagenMedicaResponse>
{

	public async ValueTask<ImagenMedicaResponse> Handle(ImagenMedicaConsultaConFiltros request, CancellationToken cancellationToken)
	{
		
		var taskModalidad = httpClient.PostAsJsonAsync(configuration["ServiciosFiltro:Modalidad"], request.Modalidad, cancellationToken: cancellationToken);
		var taskDemografia = httpClient.PostAsJsonAsync(configuration["ServiciosFiltro:Demografia"], request.Demografia, cancellationToken: cancellationToken);

		var result = await Task.WhenAll(taskModalidad, taskDemografia);
		
		var resultModalidad = await result[0].Content.ReadFromJsonAsync<IEnumerable<Guid>>(cancellationToken: cancellationToken);
		var resultDemografia = await result[1].Content.ReadFromJsonAsync<IEnumerable<Guid>>(cancellationToken: cancellationToken);
		
		var resultIds = resultModalidad.Intersect(resultDemografia).ToArray();
		
		// create an object with a property Ids and the list fo ids
		var finalRequest = new { Ids = resultIds };
		
		var imagenesResponse = await httpClient.PostAsJsonAsync(configuration["DataWarehouse:Host"], finalRequest, cancellationToken: cancellationToken);
		var imagenes = await imagenesResponse.Content.ReadFromJsonAsync<IEnumerable<ImagenDto>>(cancellationToken: cancellationToken);
		
		return new ImagenMedicaResponse { Imagenes = imagenes.Select(MapeoImagen.MapFromImagenDto).ToArray() };
	}
}