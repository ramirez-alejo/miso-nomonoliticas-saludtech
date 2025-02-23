using Consulta.Aplicacion.Demografia.Mapeo;
using Consulta.Aplicacion.Demografia.Persistencia.Entidades;
using Consulta.Aplicacion.Demografia.Persistencia.Repositorios;
using Consulta.Dominio.Eventos;

namespace Consulta.Aplicacion.Demografia.Comandos;

public class ImagenDemografiaHandler
{
    private readonly ILogger<ImagenDemografiaHandler> _logger;
    private readonly IImagenDemografiaRepository _repository;

    public ImagenDemografiaHandler(
        ILogger<ImagenDemografiaHandler> logger,
        IImagenDemografiaRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task HandleImagenDemografia(ImagenDemografiaEvent evento)
    {
        try
        {
            if (evento == null)
            {
                _logger.LogWarning("Could not deserialize message to ImagenDemografiaEvent");
                return;
            }
            
            _logger.LogInformation("Procesando Demografia de Imagen para ImagenId: {ImagenId}", evento?.ImagenId);

            var imagen = MapeoDemografiaImagen.MapFromEvent(evento);

            await _repository.UpsertAsync(imagen, CancellationToken.None);

            _logger.LogInformation("Successfully processed imagen demografia event for ImagenId: {ImagenId}",
                evento.ImagenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing imagen demografia event");
            throw;
        }
    }
}
