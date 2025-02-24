using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;
using Consulta.Dominio;
using Mediator;

namespace Consulta.Aplicacion.Modalidad.Consultas;

public class ImagenModalidadConsulta : ImagenModalidad, IRequest<ImagenModalidadResponse>
{
}

public class ImagenModalidadResponse
{
    public IEnumerable<Guid> ImagenId { get; set; }
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
        var result = await _repository.GetByCriteriaAsync(request.Modalidad?.Nombre, request.Modalidad?.Descripcion,
            request.RegionAnatomica?.Nombre, request.RegionAnatomica?.Descripcion, cancellationToken);
        return new ImagenModalidadResponse
        {
            ImagenId = result.Select(x => x.ImagenId)
        };
    }
}
