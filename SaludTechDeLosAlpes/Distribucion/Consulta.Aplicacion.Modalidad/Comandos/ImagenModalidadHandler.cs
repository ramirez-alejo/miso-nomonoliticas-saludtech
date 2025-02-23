using Consulta.Aplicacion.Modalidad.Persistencia.Entidades;
using Consulta.Aplicacion.Modalidad.Persistencia.Mapeo;
using Consulta.Aplicacion.Modalidad.Persistencia.Repositorios;
using Consulta.Dominio.Eventos;

namespace Consulta.Aplicacion.Modalidad.Comandos;

public class ImagenModalidadHandler
{
    private readonly ILogger<ImagenModalidadHandler> _logger;
    private readonly IImagenModalidadRepository _repository;

    public ImagenModalidadHandler(
        ILogger<ImagenModalidadHandler> logger,
        IImagenModalidadRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task HandleImagenModalidad(ImagenModalidadEvent evento)
    {
        try
        {
            
            if (evento == null)
            {
                _logger.LogWarning("Could not deserialize message to ImagenModalidadEvent");
                return;
            }
            
            _logger.LogInformation("Procesando Modalidad de Imagen para ImagenId: {ImagenId}", evento?.ImagenId);

            var imagen = MapeoModalidaImagen.MapFromEvent(evento);

            await _repository.UpsertAsync(imagen, CancellationToken.None);

            _logger.LogInformation("Successfully processed imagen modalidad event for ImagenId: {ImagenId}",
                evento.ImagenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing imagen modalidad event");
            throw;
        }
    }
}
