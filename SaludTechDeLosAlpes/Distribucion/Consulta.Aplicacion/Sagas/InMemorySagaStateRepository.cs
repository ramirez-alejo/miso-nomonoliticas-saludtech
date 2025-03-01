using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consulta.Aplicacion.Sagas;

public class InMemorySagaStateRepository : ISagaStateRepository
{
    private readonly ConcurrentDictionary<Guid, ImagenConsultaSagaState> _states = new();

    public Task<ImagenConsultaSagaState?> GetStateAsync(Guid sagaId)
    {
        _states.TryGetValue(sagaId, out var state);
        return Task.FromResult(state);
    }

    public Task SaveStateAsync(ImagenConsultaSagaState state)
    {
        _states[state.SagaId] = state;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ImagenConsultaSagaState>> GetPendingStatesAsync()
    {
        var pendingStates = _states.Values
            .Where(s => s.Status != "Completed" && s.Status != "Failed")
            .ToList();
        return Task.FromResult<IEnumerable<ImagenConsultaSagaState>>(pendingStates);
    }

    public Task DeleteStateAsync(Guid sagaId)
    {
        _states.TryRemove(sagaId, out _);
        return Task.CompletedTask;
    }
}
