using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Dominio;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;

public interface IImagenModalidadRepository
{
    Task<ImagenTipoImagen> GetByImagenIdAsync(Guid imagenId, CancellationToken cancellationToken);
    Task<ImagenTipoImagen> UpsertAsync(ImagenTipoImagen entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

	Task<ImagenTipoImagen[]> GetByCriteriaAsync(string nombre, string descripcion, string regionAnatomica,
		string regionDescripcion, CancellationToken cancellationToken);
}
