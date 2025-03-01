
using Ingestion.Aplicacion.Anonimizacion.Persistencia.Entidades;

namespace Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios
{
    public interface IAnonimizarRepository
    {
        Task SaveAsync(Modelos.Anonimizar anonimizar,
            CancellationToken cancellationToken);
    }
}
