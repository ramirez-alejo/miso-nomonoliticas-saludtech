using Ingestion.Aplicacion.Sagas;
using Mediator;

namespace Ingestion.Aplicacion.Consultas;

public record GetImagenIngestionSagaStateQuery(Guid SagaId) : IQuery<ImagenIngestionSagaState?>;

public class GetImagenIngestionSagaStateQueryHandler : IQueryHandler<GetImagenIngestionSagaStateQuery, ImagenIngestionSagaState?>
{
    private readonly ISagaStateRepository _stateRepository;

    public GetImagenIngestionSagaStateQueryHandler(ISagaStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async ValueTask<ImagenIngestionSagaState?> Handle(GetImagenIngestionSagaStateQuery query, CancellationToken cancellationToken)
    {
        return await _stateRepository.GetStateAsync(query.SagaId);
    }
}
