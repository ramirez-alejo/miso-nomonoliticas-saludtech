using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Consulta.Dominio.Eventos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Consulta.Aplicacion.Handlers;

public class ImagenCreadaHandler : IMessageConsumer
{
    private readonly ILogger<ImagenCreadaHandler> _logger;
    private readonly IMessageProducer _messageProducer;
    private readonly IMessageConsumer _messageConsumer;
    private const string TOPIC_DEMOGRAFIA = "imagen-demografia";
    private const string TOPIC_MODALIDAD = "imagen-modalidad";

    public ImagenCreadaHandler(
        ILogger<ImagenCreadaHandler> logger,
        IMessageProducer messageProducer,
        IMessageConsumer messageConsumer)
    {
        _logger = logger;
        _messageProducer = messageProducer;
        _messageConsumer = messageConsumer;
    }

    public async Task StartAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler)
    {
        await _messageConsumer.StartAsync(topic, subscriptionName, async (Imagen imagen) =>
        {
            try
            {
                if (imagen == null)
                {
                    _logger.LogError("Received null imagen");
                    return;
                }

                // Create and publish demographics event
                var demograficaEvent = new ImagenDemograficaEvent
                {
                    ImagenId = imagen.Id,
                    GrupoEdad = imagen.Paciente?.Demografia?.GrupoEdad ?? "No especificado",
                    Sexo = imagen.Paciente?.Demografia?.Sexo ?? "No especificado",
                    Etnicidad = imagen.Paciente?.Demografia?.Etnicidad ?? "No especificado",
                    TokenAnonimo = imagen.Paciente?.TokenAnonimo ?? "",
                    FechaCreacion = DateTime.UtcNow
                };
                await _messageProducer.SendJsonAsync(TOPIC_DEMOGRAFIA, demograficaEvent);
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
        });
    }

    public async Task StopAsync()
    {
        await _messageConsumer.StopAsync();
    }
}
