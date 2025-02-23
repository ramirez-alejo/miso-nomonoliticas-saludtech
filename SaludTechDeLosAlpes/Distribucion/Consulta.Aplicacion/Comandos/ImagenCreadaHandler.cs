using Consulta.Dominio.Eventos;
using Core.Dominio;
using Core.Infraestructura.MessageBroker;

namespace Consulta.Aplicacion.Comandos;

public class ImagenCreadaHandler
{
    private readonly ILogger<ImagenCreadaHandler> _logger;
    private readonly IMessageProducer _messageProducer;
    private const string TOPIC_DEMOGRAFIA = "imagen-demografia";
    private const string TOPIC_MODALIDAD = "imagen-modalidad";

    public ImagenCreadaHandler(
        ILogger<ImagenCreadaHandler> logger,
        IMessageProducer messageProducer)
    {
        _logger = logger;
        _messageProducer = messageProducer;
    }

    public async Task HandleImagenCreada(Imagen imagen)
    {
        try
        {
            if (imagen == null)
            {
                _logger.LogError("Received null imagen");
                return;
            }

            // Create and publish demographics event
            var demografiaEvent = new ImagenDemografiaEvent
            {
                ImagenId = imagen.Id,
                GrupoEdad = imagen.Paciente?.Demografia?.GrupoEdad ?? "No especificado",
                Sexo = imagen.Paciente?.Demografia?.Sexo ?? "No especificado",
                Etnicidad = imagen.Paciente?.Demografia?.Etnicidad ?? "No especificado",
                FechaCreacion = DateTime.UtcNow
            };
            await _messageProducer.SendJsonAsync(TOPIC_DEMOGRAFIA, demografiaEvent);
            _logger.LogInformation("Evento deDemografia publicado para la Imagen con Id {ImagenId}", imagen.Id);

            // Create and publish modalidad event
            var modalidadEvent = new ImagenModalidadEvent
            {
                ImagenId = imagen.Id,
                Nombre = imagen.TipoImagen?.Modalidad?.Nombre ?? "No especificado",
                Descripcion = imagen.TipoImagen?.Modalidad?.Descripcion ?? "No especificado",
                RegionAnatomica = imagen.TipoImagen?.RegionAnatomica?.Nombre ?? "No especificado",
                RegionDescripcion = imagen.TipoImagen?.RegionAnatomica?.Descripcion ?? "No especificado",
                FechaCreacion = DateTime.UtcNow
            };
            await _messageProducer.SendJsonAsync(TOPIC_MODALIDAD, modalidadEvent);
            _logger.LogInformation("Evento de Modalidad publicado para la Imagen con Id {ImagenId}", imagen.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing imagen message");
            throw;
        }
    }
}
