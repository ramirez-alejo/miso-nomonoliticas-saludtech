using Consulta.Aplicacion.Modalidad.Dtos;
using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Dominio.Eventos;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Mapeo;

public static class MapeoModalidaImagen
{
	public static ModalidadImagen MapToDto(ImagenModalidadEntity entity)
	{
		return new ModalidadImagen
		{
			ImagenId = entity.ImagenId,
			Modalidad = new Dtos.Modalidad
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
	
	public static ImagenModalidadEntity MapToEntity(ModalidadImagen dto)
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
	
	public static ModalidadImagen MapFromEvent(ImagenModalidadEvent evento)
	{
		return new ModalidadImagen
		{
			ImagenId = evento.ImagenId,
			Modalidad = new Dtos.Modalidad
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
