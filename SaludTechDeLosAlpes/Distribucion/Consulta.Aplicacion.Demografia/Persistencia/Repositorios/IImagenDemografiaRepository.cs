using Consulta.Aplicacion.Demografia.Dtos;
using Consulta.Aplicacion.Demografia.Persistencia.Entidades;

namespace Consulta.Aplicacion.Demografia.Persistencia.Repositorios;

public interface IImagenDemografiaRepository
{
    Task UpsertAsync(ImagenDemografia imagenDemografia, CancellationToken cancellationToken);
    Task<IEnumerable<ImagenDemografia>> BuscarPorFiltrosAsync(
        string grupoEdad = null,
        string sexo = null,
        string etnicidad = null,
        CancellationToken cancellationToken = default);
}
