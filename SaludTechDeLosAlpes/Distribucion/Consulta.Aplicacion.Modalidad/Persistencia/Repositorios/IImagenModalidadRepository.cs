using Consulta.Aplicacion.Modalidad.Dtos;
using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;

namespace Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;

public interface IImagenModalidadRepository
{
    Task<ModalidadImagen> GetByImagenIdAsync(Guid imagenId, CancellationToken cancellationToken);
    Task<ModalidadImagen> UpsertAsync(ModalidadImagen entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

	Task<ModalidadImagen[]> GetByCriteriaAsync(string nombre, string descripcion, string regionAnatomica,
		string regionDescripcion, CancellationToken cancellationToken);
}
