using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Dominio;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;

public interface IImagenModalidadRepository
{
    Task<ImagenModalidad> GetByImagenIdAsync(Guid imagenId, CancellationToken cancellationToken);
    Task<ImagenModalidad> UpsertAsync(ImagenModalidad entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

	Task<ImagenModalidad[]> GetByCriteriaAsync(string nombre, string descripcion, string regionAnatomica,
		string regionDescripcion, CancellationToken cancellationToken);
}
