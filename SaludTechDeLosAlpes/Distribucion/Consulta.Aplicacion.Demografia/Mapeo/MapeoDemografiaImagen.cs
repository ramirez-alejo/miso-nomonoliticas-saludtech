using Consulta.Aplicacion.Demografia.Persistencia.Entidades;
using Consulta.Dominio;
using Consulta.Dominio.Eventos;

namespace Consulta.Aplicacion.Demografia.Mapeo;

public static class MapeoDemografiaImagen
{
    public static ImagenDemografia MapFromEvent(ImagenDemografiaEvent evento)
    {
        return new ImagenDemografia
        {
            ImagenId = evento.ImagenId,
            GrupoEdad = evento.GrupoEdad,
            Sexo = evento.Sexo,
            Etnicidad = evento.Etnicidad,
        };
    }

    public static ImagenDemografiaEntity MapToEntity(ImagenDemografia dto)
    {
        return new ImagenDemografiaEntity
        {
            ImagenId = dto.ImagenId,
            GrupoEdad = dto.GrupoEdad,
            Sexo = dto.Sexo,
            Etnicidad = dto.Etnicidad,
        };
    }

    public static ImagenDemografia MapToDto(ImagenDemografiaEntity entity)
    {
        return new ImagenDemografia
        {
            ImagenId = entity.ImagenId,
            GrupoEdad = entity.GrupoEdad,
            Sexo = entity.Sexo,
            Etnicidad = entity.Etnicidad,
        };
    }
}
