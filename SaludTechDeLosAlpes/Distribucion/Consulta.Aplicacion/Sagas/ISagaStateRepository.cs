using System;
using System.Threading.Tasks;

namespace Consulta.Aplicacion.Sagas;

public interface ISagaStateRepository
{
    Task<ImagenConsultaSagaState?> GetStateAsync(Guid sagaId);
    Task SaveStateAsync(ImagenConsultaSagaState state);
    Task<IEnumerable<ImagenConsultaSagaState>> GetPendingStatesAsync();
    Task DeleteStateAsync(Guid sagaId);
}
