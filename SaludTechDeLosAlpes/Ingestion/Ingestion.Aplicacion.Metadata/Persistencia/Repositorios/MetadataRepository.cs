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


}