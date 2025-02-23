using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;

public interface IImagenModalidadRepository
{
    Task<ImagenModalidadEntity> GetByIdAsync(Guid id);
    Task<ImagenModalidadEntity> GetByImagenIdAsync(Guid imagenId);
    Task<IEnumerable<ImagenModalidadEntity>> GetAllAsync();
    Task<ImagenModalidadEntity> UpsertAsync(ImagenModalidadEntity entity);
    Task DeleteAsync(Guid id);
}
