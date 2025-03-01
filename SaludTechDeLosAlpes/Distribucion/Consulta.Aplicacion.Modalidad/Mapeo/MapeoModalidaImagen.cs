using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Dominio;
using Consulta.Dominio.Eventos;

namespace Consulta.Aplicacion.Modalidad.Mapeo;

public static class MapeoModalidaImagen
{
	public static ImagenTipoImagen MapToDto(ImagenModalidadEntity entity)
	{
		return new ImagenTipoImagen
		{
			ImagenId = entity.ImagenId,
			Modalidad = new Dominio.Modalidad
			{
				Nombre = entity.Nombre,
				Descripcion = entity.Descripcion
			},
			RegionAnatomica = new RegionAnatomica
			{
				Nombre = entity.RegionAnatomica,
				Descripcion = entity.RegionDescripcion
			}
		};
	}
	
	public static ImagenModalidadEntity MapToEntity(ImagenTipoImagen dto)
	{
		return new ImagenModalidadEntity
		{
			ImagenId = dto.ImagenId,
			Nombre = dto.Modalidad.Nombre,
			Descripcion = dto.Modalidad.Descripcion,
			RegionAnatomica = dto.RegionAnatomica.Nombre,
			RegionDescripcion = dto.RegionAnatomica.Descripcion
		};
	}
	
	public static ImagenTipoImagen MapFromEvent(ImagenModalidadEvent evento)
	{
		return new ImagenTipoImagen
		{
			ImagenId = evento.ImagenId,
			Modalidad = new Dominio.Modalidad
			{
				Nombre = evento.Nombre,
				Descripcion = evento.Descripcion
			},
			RegionAnatomica = new RegionAnatomica
			{
				Nombre = evento.RegionAnatomica,
				Descripcion = evento.RegionDescripcion
			}
		};
	}
}
