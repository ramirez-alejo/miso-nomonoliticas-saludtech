using Ingestion.Aplicacion.Metadata.Mapeo;
using Microsoft.EntityFrameworkCore;

namespace Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;

public class MetadataRepository : IMetadataRepository
{
	private readonly MetadataDbContext _context;
    public ILogger<MetadataRepository> _logger { get; }
	
	public MetadataRepository(MetadataDbContext context, ILogger<MetadataRepository> logger)
	{
        _logger = logger;
		_context = context;
	}
	
	//Upsert
	public async Task InsertMetadataGenerada(Modelos.Metadata modelo, CancellationToken cancellationToken)
	{
		try
        {
            var existingEntity = await _context.Imagenes
                .FirstOrDefaultAsync(x => x.ImagenId == modelo.ImagenId, cancellationToken);
            
            if (existingEntity == null)
            {
                // Create a new entity if it doesn't exist
                var newEntity = MapeoMetadataImagen.Map(modelo);
                newEntity.Id = Guid.NewGuid();
                await _context.Imagenes.AddAsync(newEntity, cancellationToken);
            }
            else
            {
                // Update the existing entity that's already being tracked
                // This approach avoids the tracking conflict
                var updatedEntity = MapeoMetadataImagen.Map(modelo);
                
                // Copy properties from updatedEntity to existingEntity
                // This keeps the same entity instance that's already being tracked
                existingEntity.ImagenId = updatedEntity.ImagenId;
                existingEntity.Version = updatedEntity.Version;
                existingEntity.VersionServicio = updatedEntity.VersionServicio;
                existingEntity.NombreModalidad = updatedEntity.NombreModalidad;
                existingEntity.DescripcionModalidad = updatedEntity.DescripcionModalidad;
                existingEntity.RegionAnatomica = updatedEntity.RegionAnatomica;
                existingEntity.DescripcionRegionAnatomica = updatedEntity.DescripcionRegionAnatomica;
                existingEntity.DescripcionPatologia = updatedEntity.DescripcionPatologia;
                existingEntity.Resolucion = updatedEntity.Resolucion;
                existingEntity.Contraste = updatedEntity.Contraste;
                existingEntity.Es3D = updatedEntity.Es3D;
                existingEntity.FaseEscaner = updatedEntity.FaseEscaner;
                existingEntity.EtapaContextoProcesal = updatedEntity.EtapaContextoProcesal;
                existingEntity.GrupoEdad = updatedEntity.GrupoEdad;
                existingEntity.Sexo = updatedEntity.Sexo;
                existingEntity.Etnicidad = updatedEntity.Etnicidad;
                existingEntity.Fumador = updatedEntity.Fumador;
                existingEntity.Diabetico = updatedEntity.Diabetico;
                existingEntity.CondicionesPrevias = updatedEntity.CondicionesPrevias;
                existingEntity.TipoAmbiente = updatedEntity.TipoAmbiente;
                existingEntity.Sintomas = updatedEntity.Sintomas;
                existingEntity.Timestamp = updatedEntity.Timestamp;
                
                // No need to call Update() as the entity is already being tracked
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar la información demográfica de la imagen");
            throw;
        }
	}
	
	public async Task DeleteMetadataGenerada(Guid imagenId, CancellationToken cancellationToken)
	{
		try
        {
            var entity = await _context.Imagenes
                .FirstOrDefaultAsync(x => x.ImagenId == imagenId, cancellationToken);
            
            if (entity != null)
            {
                _context.Imagenes.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted metadata for imagen {ImagenId}", imagenId);
            }
            else
            {
                _logger.LogWarning("No metadata found for imagen {ImagenId} to delete", imagenId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata for imagen {ImagenId}", imagenId);
            throw;
        }
	}
}
