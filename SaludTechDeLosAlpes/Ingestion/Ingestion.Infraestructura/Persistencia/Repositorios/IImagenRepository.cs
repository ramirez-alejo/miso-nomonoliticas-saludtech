using Core.Dominio;
using Ingestion.Infraestructura.Persistencia.Entidades;

namespace Ingestion.Infraestructura.Persistencia.Repositorios;

public interface IImagenRepository
{
    Task<Imagen> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Imagen> AddAsync(Imagen imagen, CancellationToken cancellationToken);
    Task UpdateAsync(Imagen imagen, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<Imagen>> GetByModalidadAndDemografiaAsync(Modalidad modalidad, Demografia demografia,
        CancellationToken cancellationToken);
    Task<IEnumerable<Imagen>> GetByRegionAnatomicaAsync(string regionAnatomica, CancellationToken cancellationToken);
    Task<Imagen[]> GetByIdsAsync(Guid[] ids, CancellationToken cancellationToken);
    Task UpsertMetadataGenerada(Guid imagenId, Dictionary<string, string> tags, CancellationToken cancellationToken);
    Task UpsertImagenAnonimizada(Guid imagenId, string ubicacionImagenProcesada, CancellationToken cancellationToken);
    
    // Compensation methods for saga
    Task DeleteAnonimizacionData(Guid imagenId, CancellationToken cancellationToken);
    Task DeleteMetadataData(Guid imagenId, CancellationToken cancellationToken);
}
