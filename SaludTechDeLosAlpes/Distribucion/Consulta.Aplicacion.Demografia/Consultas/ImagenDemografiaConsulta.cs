using Consulta.Aplicacion.Demografia.Persistencia.Entidades;
using Consulta.Aplicacion.Demografia.Persistencia.Repositorios;
using Mediator;

namespace Consulta.Aplicacion.Demografia.Consultas;

public class ImagenDemografiaConsulta : IRequest<ImagenDemografiaResponse>
{
    public string GrupoEdad { get; set; }

    // E.g., "Masculino", "Femenino", "Intersexual"
    public string Sexo { get; set; }

    // E.g., "Latino", "Caucásico", "Afro", "Asiático"
    public string Etnicidad { get; set; }
}

public record ImagenDemografiaResponse
{
    public IEnumerable<Guid> ImagenIds { get; set; }
}

public class ImagenDemografiaConsultaHandler : IRequestHandler<ImagenDemografiaConsulta, ImagenDemografiaResponse>
{
    private readonly IImagenDemografiaRepository _repository;
    private readonly ILogger<ImagenDemografiaConsultaHandler> _logger;

    public ImagenDemografiaConsultaHandler(
        IImagenDemografiaRepository repository,
        ILogger<ImagenDemografiaConsultaHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async ValueTask<ImagenDemografiaResponse> Handle(ImagenDemografiaConsulta request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _repository.BuscarPorFiltrosAsync(
                request.GrupoEdad,
                request.Sexo,
                request.Etnicidad,
                cancellationToken);
            
            return new ImagenDemografiaResponse
            {
                ImagenIds = result.Select(x => x.ImagenId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar información demográfica de imágenes");
            throw;
        }
    }
}
