using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;

public class ImagenModalidadRepository : IImagenModalidadRepository
{
    private readonly ImagenModalidadDbContext _context;

    public ImagenModalidadRepository(ImagenModalidadDbContext context)
    {
        _context = context;
    }

    public async Task<ImagenModalidadEntity> GetByIdAsync(Guid id)
    {
        return await _context.ImagenesModalidad.FindAsync(id);
    }

    public async Task<ImagenModalidadEntity> GetByImagenIdAsync(Guid imagenId)
    {
        return await _context.ImagenesModalidad
            .FirstOrDefaultAsync(x => x.ImagenId == imagenId);
    }

    public async Task<IEnumerable<ImagenModalidadEntity>> GetAllAsync()
    {
        return await _context.ImagenesModalidad.ToListAsync();
    }

    public async Task<ImagenModalidadEntity> UpsertAsync(ImagenModalidadEntity entity)
    {
        var existing = await _context.ImagenesModalidad
            .FirstOrDefaultAsync(x => x.ImagenId == entity.ImagenId);

        if (existing != null)
        {
            // Update existing entity
            existing.Nombre = entity.Nombre;
            existing.Descripcion = entity.Descripcion;
            existing.RegionAnatomica = entity.RegionAnatomica;
            existing.RegionDescripcion = entity.RegionDescripcion;
            existing.FechaCreacion = entity.FechaCreacion;
            _context.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            // Add new entity
            _context.ImagenesModalidad.Add(entity);
        }

        await _context.SaveChangesAsync();
        return existing ?? entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.ImagenesModalidad.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
