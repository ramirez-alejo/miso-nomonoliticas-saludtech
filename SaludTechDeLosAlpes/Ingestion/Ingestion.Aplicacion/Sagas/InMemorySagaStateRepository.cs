using System.Collections.Concurrent;
using Ingestion.Dominio.Saga;

namespace Ingestion.Aplicacion.Sagas;

public class InMemorySagaStateRepository : ISagaStateRepository
{
    private readonly ConcurrentDictionary<Guid, ImagenIngestionSagaState> _states = new();

    public Task<ImagenIngestionSagaState?> GetStateAsync(Guid sagaId)
    {
        _states.TryGetValue(sagaId, out var state);
        return Task.FromResult(state);
    }

    public Task SaveStateAsync(ImagenIngestionSagaState state)
    {
        _states[state.SagaId] = state;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ImagenIngestionSagaState>> GetPendingStatesAsync()
    {
        var pendingStates = _states.Values
            .Where(s => s.Status != "Completed" && s.Status != "Failed")
            .ToList();
        return Task.FromResult<IEnumerable<ImagenIngestionSagaState>>(pendingStates);
    }

    public Task DeleteStateAsync(Guid sagaId)
    {
        _states.TryRemove(sagaId, out _);
        return Task.CompletedTask;
    }
}
