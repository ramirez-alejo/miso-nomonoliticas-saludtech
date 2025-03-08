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
	public async Task UpsertMetadataGenerada(Modelos.Metadata modelo, CancellationToken cancellationToken)
	{
		try
        {
            var existingEntity = await _context.Imagenes
                .FirstOrDefaultAsync(x => x.ImagenId == modelo.ImagenId, cancellationToken);
            
            var entity = MapeoMetadataImagen.Map(modelo);

            if (existingEntity == null)
            {
                entity.Id = Guid.NewGuid();
                await _context.Imagenes.AddAsync(entity, cancellationToken);
            }
            else
            {
				entity.Id = existingEntity.Id;
				_context.Imagenes.Update(entity);
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
