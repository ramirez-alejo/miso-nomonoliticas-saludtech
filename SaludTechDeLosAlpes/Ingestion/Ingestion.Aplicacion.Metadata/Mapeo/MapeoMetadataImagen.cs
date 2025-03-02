using Avro.Util;
using Ingestion.Aplicacion.Metadata.Persistencia.Entidades;

namespace Ingestion.Aplicacion.Metadata.Mapeo;

public static class MapeoMetadataImagen
{
	public static MetadataImagenEntity Map(Modelos.Metadata modelo)
	{
		return new MetadataImagenEntity
		{
			Version = modelo.Version,
			ImagenId = modelo.ImagenId,
			VersionServicio = "1.0.0",
			NombreModalidad = modelo.TipoImagen?.Modalidad?.Nombre,
			DescripcionModalidad = modelo.TipoImagen?.Modalidad?.Descripcion,
			RegionAnatomica = modelo.TipoImagen?.RegionAnatomica?.Nombre,
			DescripcionRegionAnatomica = modelo.TipoImagen?.RegionAnatomica?.Descripcion,
			DescripcionPatologia = modelo.TipoImagen?.Patologia?.Descripcion,
			Resolucion = modelo.Resolucion,
			Contraste = modelo.Contraste,
			Es3D = modelo.Es3D,
			FaseEscaner = modelo.FaseEscaner,
			EtapaContextoProcesal = modelo.ContextoProcesal?.Etapa,
			GrupoEdad = modelo.Demografia?.GrupoEdad,
			Sexo = modelo.Demografia?.Sexo,
			Etnicidad = modelo.Demografia?.Etnicidad,
			Fumador = modelo.Historial?.Fumador ?? false,
			Diabetico = modelo.Historial?.Diabetico ?? false,
			CondicionesPrevias = string.Join('|', modelo.Historial?.CondicionesPrevias ?? []),
			TipoAmbiente = modelo.EntornoClinico?.TipoAmbiente,
			Sintomas = string.Join('|', modelo.Sintomas?.Select(s => s.Descripcion) ?? []),
			Timestamp = DateTime.UtcNow
		};
	}

	public static Dominio.Eventos.MetadataGenerada MapToEvent(Modelos.Metadata modelo)
	{
		return new Dominio.Eventos.MetadataGenerada
		{
			Version = modelo.Version,
			ImagenId = modelo.ImagenId,
			Tags = modelo.Tags,
			Timestamp = DateTime.UtcNow
		};
	}
	
	public static Modelos.Metadata MapFromCommand(Dominio.Comandos.GenerarMetadata comando)
	{
		return new Modelos.Metadata
		{
			Version = comando.Version,
			ImagenId = comando.ImagenId,
			TipoImagen = new Core.Dominio.TipoImagen
			{
				Modalidad = new Core.Dominio.Modalidad
				{
					Nombre = comando.TipoImagen?.Modalidad?.Nombre,
					Descripcion = comando.TipoImagen?.Modalidad?.Descripcion
				},
				RegionAnatomica = new Core.Dominio.RegionAnatomica
				{
					Nombre = comando.TipoImagen?.RegionAnatomica?.Nombre,
					Descripcion = comando.TipoImagen?.RegionAnatomica?.Descripcion
				},
				Patologia = new Core.Dominio.Patologia
				{
					Descripcion = comando.TipoImagen?.Patologia?.Descripcion
				}
			},
			Resolucion = comando.Resolucion,
			Contraste = comando.Contraste,
			Es3D = comando.Es3D,
			FaseEscaner = comando.FaseEscaner,
			ContextoProcesal = new Core.Dominio.ContextoProcesal
			{
				Etapa = comando.ContextoProcesal?.Etapa
			},
			Demografia = new Core.Dominio.Demografia
			{
				GrupoEdad = comando.Demografia?.GrupoEdad,
				Sexo = comando.Demografia?.Sexo,
				Etnicidad = comando.Demografia?.Etnicidad
			},
			Historial = new Core.Dominio.Historial
			{
				Fumador = comando.Historial?.Fumador ?? false,
				Diabetico = comando.Historial?.Diabetico ?? false,
				CondicionesPrevias = comando.Historial?.CondicionesPrevias
			},
			EntornoClinico = new Core.Dominio.EntornoClinico
			{
				TipoAmbiente = comando.EntornoClinico?.TipoAmbiente
			},
			Sintomas = comando.Sintomas,
			Tags = new Dictionary<string, string>()
		};
	}
}

			
			