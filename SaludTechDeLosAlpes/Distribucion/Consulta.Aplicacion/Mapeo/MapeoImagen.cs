using Consulta.Aplicacion.Dtos;
using Consulta.Dominio;
using AtributosImagen = Core.Dominio.AtributosImagen;
using ContextoProcesal = Core.Dominio.ContextoProcesal;
using Demografia = Core.Dominio.Demografia;
using EntornoClinico = Core.Dominio.EntornoClinico;
using Metadatos = Core.Dominio.Metadatos;
using Modalidad = Core.Dominio.Modalidad;
using Patologia = Core.Dominio.Patologia;
using RegionAnatomica = Core.Dominio.RegionAnatomica;
using Sintoma = Core.Dominio.Sintoma;

namespace Consulta.Aplicacion.Mapeo;

public static class MapeoImagen
{
	public static ImagenMedica MapFromImagenDto(ImagenDto imagenDto)
	{
		return new ImagenMedica
		{
			Id = imagenDto.Id,
			Version = imagenDto.Version,
			TipoImagen = new Core.Dominio.TipoImagen
			{
				Modalidad = new Modalidad
				{
					Nombre = imagenDto.TipoImagen?.Modalidad?.Nombre,
					Descripcion = imagenDto.TipoImagen?.Modalidad?.Descripcion
				},
				RegionAnatomica = new RegionAnatomica
				{
					Nombre = imagenDto.TipoImagen?.RegionAnatomica?.Nombre,
					Descripcion = imagenDto.TipoImagen?.RegionAnatomica?.Descripcion
				},
				Patologia = new Patologia
				{
					Descripcion = imagenDto.TipoImagen?.Patologia?.Descripcion
				}
			},
			AtributosImagen = new AtributosImagen
			{
				Resolucion = imagenDto.AtributosImagen?.Resolucion,
				Contraste = imagenDto.AtributosImagen?.Contraste,
				Es3D = imagenDto.AtributosImagen?.Es3D ?? false,
				FaseEscaner = imagenDto.AtributosImagen?.FaseEscaner
			},
			ContextoProcesal = new ContextoProcesal
			{
				Etapa = imagenDto.ContextoProcesal?.Etapa
			},
			Metadatos = new Metadatos
			{
				EntornoClinico = new EntornoClinico
				{
					TipoAmbiente = imagenDto.Metadatos?.EntornoClinico?.TipoAmbiente
				},
				Sintomas = imagenDto.Metadatos?.Sintomas?.Select(s => new Sintoma
					{
						Descripcion = s?.Descripcion
					}).ToList() ?? new List<Sintoma>()
			},
			Demografia = new Demografia
			{
				Etnicidad = imagenDto.Paciente?.Demografia?.Etnicidad,
				GrupoEdad = imagenDto.Paciente?.Demografia?.GrupoEdad,
				Sexo = imagenDto.Paciente?.Demografia?.Sexo
			}
		};
	}
}
