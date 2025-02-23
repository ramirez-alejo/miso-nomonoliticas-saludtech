using Core.Dominio;
using Ingestion.Infraestructura.Persistencia.Entidades;

namespace Ingestion.Infraestructura.Persistencia.Repositorios;

public interface IImagenRepository
{
    Task<Imagen> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Imagen> AddAsync(Imagen imagen, CancellationToken cancellationToken);
    Task UpdateAsync(Imagen imagen, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Imagen>> GetByDemografiaAsync(string grupoEdad, string sexo, string etnicidad,
        CancellationToken cancellationToken);
    Task<IEnumerable<Imagen>> GetByModalidadAsync(string modalidad, CancellationToken cancellationToken);
    Task<IEnumerable<Imagen>> GetByRegionAnatomicaAsync(string regionAnatomica, CancellationToken cancellationToken);
}
