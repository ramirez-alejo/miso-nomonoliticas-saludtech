using System.Text.Json;
using Core.Dominio;
using Ingestion.Aplicacion.Comandos;
using Ingestion.Aplicacion.Dtos;
using Ingestion.Dominio.Eventos;
using Ingestion.Infraestructura.Persistencia.Entidades;

namespace Ingestion.Aplicacion.Mapeo;

public static class MapeoImagen
{
    public static ImagenDto MapToDto(ImagenEntity entity)
    {
        return new ImagenDto
        {
            Id = entity.Id,
            Nombre = $"Imagen {entity.Id}",
            Descripcion = $"Modalidad: {entity.TipoImagen?.Modalidad?.Nombre}, Región: {entity.TipoImagen?.RegionAnatomica?.Nombre}",
            FechaCreacion = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)), // Simulated for now
            Url = $"https://datawarehouse.saludtech.com/imagenes/{entity.Id}.jpg"
        };
    }

    public static Imagen MapToCore(ImagenEntity entity)
    {
        return new Imagen
        {
            Id = entity.Id,
            Version = entity.Version,
            TipoImagen = new TipoImagen
            {
                Modalidad = new Modalidad
                {
                    Nombre = entity.TipoImagen?.Modalidad?.Nombre,
                    Descripcion = entity.TipoImagen?.Modalidad?.Descripcion
                },
                RegionAnatomica = new RegionAnatomica
                {
                    Nombre = entity.TipoImagen?.RegionAnatomica?.Nombre,
                    Descripcion = entity.TipoImagen?.RegionAnatomica?.Descripcion
                },
                Patologia = new Patologia
                {
                    Descripcion = entity.TipoImagen?.Patologia?.Descripcion
                }
            },
            AtributosImagen = new AtributosImagen
            {
                Resolucion = entity.AtributosImagen?.Resolucion,
                Contraste = entity.AtributosImagen?.Contraste,
                Es3D = entity.AtributosImagen?.Es3D ?? false,
                FaseEscaner = entity.AtributosImagen?.FaseEscaner
            },
            ContextoProcesal = new ContextoProcesal
            {
                Etapa = entity.ContextoProcesal?.Etapa
            },
            Metadatos = new Metadatos
            {
                EntornoClinico = new EntornoClinico
                {
                    TipoAmbiente = entity.Metadatos?.EntornoClinico?.TipoAmbiente
                },
                Sintomas = entity.Metadatos?.Sintomas?.Select(s => new Sintoma
                {
                    Descripcion = s.Descripcion
                }).ToList() ?? new List<Sintoma>()
            },
            Paciente = new Paciente
            {
                Demografia = new Demografia
                {
                    GrupoEdad = entity.Paciente?.Demografia?.GrupoEdad,
                    Sexo = entity.Paciente?.Demografia?.Sexo,
                    Etnicidad = entity.Paciente?.Demografia?.Etnicidad
                },
                Historial = new Historial
                {
                    Fumador = entity.Paciente?.Historial?.Fumador ?? false,
                    Diabetico = entity.Paciente?.Historial?.Diabetico ?? false,
                    CondicionesPrevias = entity.Paciente?.Historial?.CondicionesPrevias?.Split(',').ToList() ?? new List<string>()
                },
                TokenAnonimo = entity.Paciente?.TokenAnonimo
            }
        };
    }

    public static CrearImagenMedicaCommand MapToCommand(SolicitudProcesamientoImagen solicitudProcesamientoImagen)
    {
        return new CrearImagenMedicaCommand
        {
            Id = solicitudProcesamientoImagen.Id,
            Version = solicitudProcesamientoImagen.Version,
            TipoImagen = solicitudProcesamientoImagen.TipoImagen,
            AtributosImagen = solicitudProcesamientoImagen.AtributosImagen,
            ContextoProcesal = solicitudProcesamientoImagen.ContextoProcesal,
            Metadatos = solicitudProcesamientoImagen.Metadatos,
            Paciente = solicitudProcesamientoImagen.Paciente
        };
    }
}
