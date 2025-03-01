using Consulta.Aplicacion.Sagas;
using Mediator;

namespace Consulta.Aplicacion.Consultas;

public record GetImagenConsultaSagaStateQuery(Guid SagaId) : IRequest<ImagenConsultaSagaState?>;

public class GetImagenConsultaSagaStateQueryHandler : IRequestHandler<GetImagenConsultaSagaStateQuery, ImagenConsultaSagaState?>
{
    private readonly ISagaStateRepository _stateRepository;
    private readonly ILogger<GetImagenConsultaSagaStateQueryHandler> _logger;

    public GetImagenConsultaSagaStateQueryHandler(
        ISagaStateRepository stateRepository,
        ILogger<GetImagenConsultaSagaStateQueryHandler> logger)
    {
        _stateRepository = stateRepository;
        _logger = logger;
    }

    public async ValueTask<ImagenConsultaSagaState?> Handle(GetImagenConsultaSagaStateQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting imagen consulta saga state for saga {SagaId}", query.SagaId);
        return await _stateRepository.GetStateAsync(query.SagaId);
    }
}
