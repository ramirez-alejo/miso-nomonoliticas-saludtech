using Core.Dominio;
using Ingestion.Infraestructura.Persistencia.Entidades;

namespace Ingestion.Infraestructura.Mapeo;

public static class MapeoImagen
{
	public static Imagen MapearImagenEntityADominio(Ingestion.Infraestructura.Persistencia.Entidades.ImagenEntity imagenEntity)
	{
		return new Imagen
        {
            Id = imagenEntity.Id,
            Version = imagenEntity.Version,
            TipoImagen = new TipoImagen
            {
                Modalidad = new Modalidad
                {
                    Nombre = imagenEntity.TipoImagen?.Modalidad?.Nombre,
                    Descripcion = imagenEntity.TipoImagen?.Modalidad?.Descripcion
                },
                RegionAnatomica = new RegionAnatomica
                {
                    Nombre = imagenEntity.TipoImagen?.RegionAnatomica?.Nombre,
                    Descripcion = imagenEntity.TipoImagen?.RegionAnatomica?.Descripcion
                },
                Patologia = new Patologia
                {
                    Descripcion = imagenEntity.TipoImagen?.Patologia?.Descripcion
                }
            },
            AtributosImagen = new AtributosImagen
            {
                Resolucion = imagenEntity.AtributosImagen?.Resolucion,
                Contraste = imagenEntity.AtributosImagen?.Contraste,
                Es3D = imagenEntity.AtributosImagen?.Es3D ?? false,
                FaseEscaner = imagenEntity.AtributosImagen?.FaseEscaner
            },
            ContextoProcesal = new ContextoProcesal
            {
                Etapa = imagenEntity.ContextoProcesal?.Etapa
            },
            Metadatos = new Metadatos
            {
                EntornoClinico = new EntornoClinico
                {
                    TipoAmbiente = imagenEntity.Metadatos?.EntornoClinico?.TipoAmbiente
                },
                Sintomas = imagenEntity.Metadatos?.Sintomas?.Select(s => new Sintoma
                {
                    Descripcion = s?.Descripcion
                }).ToList()
            },
            Paciente = new Paciente
            {
                Demografia = new Demografia
                {
                    GrupoEdad = imagenEntity.Paciente?.Demografia?.GrupoEdad,
                    Sexo = imagenEntity.Paciente?.Demografia?.Sexo,
                    Etnicidad = imagenEntity.Paciente?.Demografia?.Etnicidad
                },
                Historial = new Historial
                {
                    Fumador = imagenEntity.Paciente?.Historial?.Fumador ?? false,
                    Diabetico = imagenEntity.Paciente?.Historial?.Diabetico ?? false,
                    CondicionesPrevias = imagenEntity.Paciente?.Historial?.CondicionesPrevias?.Split(',')?.ToList()
                },
                TokenAnonimo = imagenEntity.Paciente?.TokenAnonimo,
            },
        };
	}

    public static Ingestion.Infraestructura.Persistencia.Entidades.ImagenEntity MapearImagenDominioAEntity(
        Imagen imagen)
    {
        return new ImagenEntity
        {
            Id = imagen.Id,
            Version = imagen.Version,
            TipoImagen = new TipoImagenEntity
            {
                Modalidad = new ModalidadEntity
                {
                    Nombre = imagen.TipoImagen?.Modalidad?.Nombre,
                    Descripcion = imagen.TipoImagen?.Modalidad?.Descripcion
                },
                RegionAnatomica = new RegionAnatomicaEntity
                {
                    Nombre = imagen.TipoImagen?.RegionAnatomica?.Nombre,
                    Descripcion = imagen.TipoImagen?.RegionAnatomica?.Descripcion
                },
                Patologia = new PatologiaEntity
                {
                    Descripcion = imagen.TipoImagen?.Patologia?.Descripcion
                }
            },
            AtributosImagen = new AtributosImagenEntity
            {
                Resolucion = imagen.AtributosImagen?.Resolucion,
                Contraste = imagen.AtributosImagen?.Contraste,
                Es3D = imagen.AtributosImagen?.Es3D ?? false,
                FaseEscaner = imagen.AtributosImagen?.FaseEscaner
            },
            ContextoProcesal = new ContextoProcesalEntity
            {
                Etapa = imagen.ContextoProcesal?.Etapa
            },
            Metadatos = new MetadatosEntity
            {
                EntornoClinico = new EntornoClinicoEntity
                {
                    TipoAmbiente = imagen.Metadatos?.EntornoClinico?.TipoAmbiente
                },
                Sintomas = imagen.Metadatos?.Sintomas?.Select(s => new SintomaEntity
                {
                    Descripcion = s.Descripcion
                })?.ToList()
            },
            Paciente = new PacienteEntity
            {
                Demografia = new DemografiaEntity
                {
                    GrupoEdad = imagen.Paciente?.Demografia?.GrupoEdad,
                    Sexo = imagen.Paciente?.Demografia?.Sexo,
                    Etnicidad = imagen.Paciente?.Demografia?.Etnicidad
                },
                Historial = new HistorialEntity
                {
                    Fumador = imagen.Paciente?.Historial?.Fumador ?? false,
                    Diabetico = imagen.Paciente?.Historial?.Diabetico ?? false,
                    CondicionesPrevias = string.Join(',', imagen.Paciente?.Historial?.CondicionesPrevias)
                },
                TokenAnonimo = imagen.Paciente?.TokenAnonimo,
            },
        };

    }
}