using Consulta.Aplicacion.Demografia.Persistencia.Entidades;
using Consulta.Dominio;

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
