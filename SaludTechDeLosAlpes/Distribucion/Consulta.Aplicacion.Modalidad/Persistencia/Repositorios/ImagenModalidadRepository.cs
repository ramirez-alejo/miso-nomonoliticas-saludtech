using Consulta.Aplicacion.Modalidad.Mapeo;
using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Dominio;
using Microsoft.EntityFrameworkCore;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;

public class ImagenModalidadRepository : IImagenModalidadRepository
{
	private readonly ImagenModalidadDbContext _context;

	public ImagenModalidadRepository(ImagenModalidadDbContext context)
	{
		_context = context;
	}

	public async Task<ImagenTipoImagen[]> GetByCriteriaAsync(string nombre, string descripcion, string regionAnatomica,
		string regionDescripcion, CancellationToken cancellationToken)
	{
		var queryBuilder = _context.ImagenesModalidad.AsQueryable();
		if (!string.IsNullOrWhiteSpace(nombre))
		{
			queryBuilder = queryBuilder.Where(x => x.Nombre.Contains(nombre));
		}

		if (!string.IsNullOrWhiteSpace(descripcion))
		{
			queryBuilder = queryBuilder.Where(x => x.Descripcion.Contains(descripcion));
		}

		if (!string.IsNullOrWhiteSpace(regionAnatomica))
		{
			queryBuilder = queryBuilder.Where(x => x.RegionAnatomica.Contains(regionAnatomica));
		}

		if (!string.IsNullOrWhiteSpace(regionDescripcion))
		{
			queryBuilder = queryBuilder.Where(x => x.RegionDescripcion.Contains(regionDescripcion));
		}

		var result = await queryBuilder.ToArrayAsync(cancellationToken);
		return result.Select(MapeoModalidaImagen.MapToDto).ToArray();
	}


	public async Task<ImagenTipoImagen> GetByImagenIdAsync(Guid imagenId, CancellationToken cancellationToken)
	{
		var result = await _context.ImagenesModalidad
			.FirstOrDefaultAsync(x => x.ImagenId == imagenId, cancellationToken: cancellationToken);
		return MapeoModalidaImagen.MapToDto(result);
	}

	public async Task<ImagenTipoImagen> UpsertAsync(ImagenTipoImagen modailidad,
		CancellationToken cancellationToken)
	{
		var existing = await _context.ImagenesModalidad
			.FirstOrDefaultAsync(x => x.ImagenId == modailidad.ImagenId, cancellationToken: cancellationToken);

		var entity = MapeoModalidaImagen.MapToEntity(modailidad);
		if (existing == null)
		{
			await _context.ImagenesModalidad.AddAsync(entity, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return MapeoModalidaImagen.MapToDto(entity);
		}

		entity.Id = existing.Id;
		_context.ImagenesModalidad.Update(entity);
		await _context.SaveChangesAsync(cancellationToken);
		return MapeoModalidaImagen.MapToDto(entity);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var entity = await _context.ImagenesModalidad
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
		if (entity != null)
		{
			_context.ImagenesModalidad.Remove(entity);
			await _context.SaveChangesAsync(cancellationToken);
		}
	}
}