using Consulta.Aplicacion.Demografia.Mapeo;
using Consulta.Aplicacion.Demografia.Persistencia.Entidades;
using Consulta.Dominio;
using Microsoft.EntityFrameworkCore;

namespace Consulta.Aplicacion.Demografia.Persistencia.Repositorios;

public class ImagenDemografiaRepository : IImagenDemografiaRepository
{
    private readonly ImagenDemografiaDbContext _context;
    private readonly ILogger<ImagenDemografiaRepository> _logger;

    public ImagenDemografiaRepository(
        ImagenDemografiaDbContext context,
        ILogger<ImagenDemografiaRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task UpsertAsync(ImagenDemografia imagenDemografia, CancellationToken cancellationToken)
    {
        try
        {
            var existingEntity = await _context.ImagenesDemograficas
                .FirstOrDefaultAsync(x => x.ImagenId == imagenDemografia.ImagenId, cancellationToken);
            
            var entity = MapeoDemografiaImagen.MapToEntity(imagenDemografia);

            if (existingEntity == null)
            {
                entity.Id = Guid.NewGuid();
                await _context.ImagenesDemograficas.AddAsync(entity, cancellationToken);
            }
            else
            {
                existingEntity.GrupoEdad = entity.GrupoEdad;
                existingEntity.Sexo = entity.Sexo;
                existingEntity.Etnicidad = entity.Etnicidad;
                _context.ImagenesDemograficas.Update(existingEntity);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar la información demográfica de la imagen");
            throw;
        }
    }

    public async Task<IEnumerable<ImagenDemografia>> BuscarPorFiltrosAsync(
        string grupoEdad = null,
        string sexo = null,
        string etnicidad = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.ImagenesDemograficas.AsQueryable();

            if (!string.IsNullOrEmpty(grupoEdad))
                query = query.Where(x => x.GrupoEdad.Contains(grupoEdad));

            if (!string.IsNullOrEmpty(sexo))
                query = query.Where(x => x.Sexo.Contains(sexo));

            if (!string.IsNullOrEmpty(etnicidad))
                query = query.Where(x => x.Etnicidad.Contains(etnicidad));

            var result = await query.ToListAsync(cancellationToken);
            return result.Select(MapeoDemografiaImagen.MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar información demográfica de imágenes");
            throw;
        }
    }
}
