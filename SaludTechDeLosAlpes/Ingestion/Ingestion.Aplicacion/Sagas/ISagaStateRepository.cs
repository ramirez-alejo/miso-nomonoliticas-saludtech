namespace Ingestion.Aplicacion.Sagas;

public interface ISagaStateRepository
{
    Task<ImagenIngestionSagaState?> GetStateAsync(Guid sagaId);
    Task SaveStateAsync(ImagenIngestionSagaState state);
    Task<IEnumerable<ImagenIngestionSagaState>> GetPendingStatesAsync();
    Task DeleteStateAsync(Guid sagaId);
}
