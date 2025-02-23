using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;
using Mediator;

namespace Consulta.Aplicacion.Modalidad.Consultas;

public class ImagenModalidadConsulta : IRequest<ImagenModalidadResponse>
{
    public Guid? Id { get; set; }
    public Guid? ImagenId { get; set; }
}

public class ImagenModalidadResponse
{
    public IEnumerable<ImagenModalidadEntity> Imagenes { get; set; }
}

public class ImagenModalidadConsultaHandler : IRequestHandler<ImagenModalidadConsulta, ImagenModalidadResponse>
{
    private readonly IImagenModalidadRepository _repository;
    private readonly ILogger<ImagenModalidadConsultaHandler> _logger;

    public ImagenModalidadConsultaHandler(
        IImagenModalidadRepository repository,
        ILogger<ImagenModalidadConsultaHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async ValueTask<ImagenModalidadResponse> Handle(ImagenModalidadConsulta request, CancellationToken cancellationToken)
    {
        if (request.Id.HasValue)
        {
            var result = await _repository.GetByIdAsync(request.Id.Value);
            _logger.LogInformation("Imagen modalidad consultada por id: {Id}", request.Id.Value);
            return new ImagenModalidadResponse { Imagenes = result != null ? new[] { result } : Array.Empty<ImagenModalidadEntity>() };
        }

        if (request.ImagenId.HasValue)
        {
            var result = await _repository.GetByImagenIdAsync(request.ImagenId.Value);
            _logger.LogInformation("Imagen modalidad consultada por imagen id: {ImagenId}", request.ImagenId.Value);
            return new ImagenModalidadResponse { Imagenes = result != null ? new[] { result } : Array.Empty<ImagenModalidadEntity>() };
        }

        var allResults = await _repository.GetAllAsync();
        _logger.LogInformation("Todas las imagenes modalidad consultadas");
        return new ImagenModalidadResponse { Imagenes = allResults };
    }
}
