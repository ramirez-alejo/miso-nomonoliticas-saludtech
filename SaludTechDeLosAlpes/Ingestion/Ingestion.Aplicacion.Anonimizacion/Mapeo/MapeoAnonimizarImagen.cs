using Core.Dominio;
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
                Resolucion = model.AtributosImagen?.Resolucion,
                Contraste = model.AtributosImagen?.Contraste,
                Es3D = model.AtributosImagen?.Es3D ?? false,
                FaseEscaner = model.AtributosImagen?.FaseEscaner,
                UbicacionImagen = model.UbicacionImagen,
            };
        }

        public static Modelos.Anonimizar MapToModel(Anonimizar dto)
        {
            return new Modelos.Anonimizar
            {
                ImagenId = dto.ImagenId,
                TipoImagen = new TipoImagen
                {
                    Modalidad = new Modalidad
                    {
                        Nombre = dto.TipoImagen?.Modalidad?.Nombre,
                        Descripcion = dto.TipoImagen?.Modalidad?.Descripcion,
                    },
                    RegionAnatomica = new RegionAnatomica
                    {
                        Nombre = dto.TipoImagen?.RegionAnatomica?.Nombre,
                        Descripcion = dto.TipoImagen?.RegionAnatomica?.Descripcion,
                    },
                },
                AtributosImagen = new AtributosImagen
                {
                    Resolucion = dto.Resolucion,
                    Contraste = dto.Contraste,
                    Es3D = dto.Es3D,
                    FaseEscaner = dto.FaseEscaner,
                },
                UbicacionImagen = dto.UbicacionImagen,
            };
        }
    }
}
