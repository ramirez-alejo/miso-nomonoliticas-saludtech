using Consulta.Dominio;
using Consulta.Dominio.Eventos;
using Ingestion.Aplicacion.Anonimizacion.Persistencia.Entidades;
using Ingestion.Dominio.Comandos;

namespace Ingestion.Aplicacion.Anonimizacion.Mapeo
{
    public static class MapeoAnonimizarImagen
    {
        public static ImagenAnonimizadaEntity MapFromModel(Modelos.Anonimizar model)
        {
            return new ImagenAnonimizadaEntity
            {
                ImagenId = model.ImagenId,
                DescripcionModalidad = model.TipoImagen.Modalidad.Descripcion,
                NombreModalidad = model.TipoImagen.Modalidad.Nombre,
                RegionAnatomica = model.TipoImagen.RegionAnatomica.Nombre,
                DescripcionRegionAnatomica = model.TipoImagen.RegionAnatomica.Descripcion,
                Resolucion = model.Resolucion,
                Contraste = model.Contraste,
                Es3D = model.Es3D,
                FaseEscaner = model.FaseEscaner,
                UbicacionImagen = model.UbicacionImagen,
            };
        }

        public static Modelos.Anonimizar MapToModel(Anonimizar dto)
        {
            return new Modelos.Anonimizar
            {
                ImagenId = dto.ImagenId,
                TipoImagen = dto.TipoImagen,
                AtributosImagen = dto.AtributosImagen,
                Resolucion = dto.Resolucion,
                Contraste = dto.Contraste,
                Es3D = dto.Es3D,
                FaseEscaner = dto.FaseEscaner,
                UbicacionImagen = dto.UbicacionImagen,
            };
        }
    }
}
