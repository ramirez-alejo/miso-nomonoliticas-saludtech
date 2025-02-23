using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Consulta.Aplicacion.Modalidad.Comandos;
using Consulta.Dominio.Eventos;

namespace Consulta.Aplicacion.Modalidad.Workers;

public class ImagenModalidadSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenModalidadSubscriptionWorker> _logger;
    private readonly IMessageConsumer _messageConsumer;
    private readonly ImagenModalidadHandler _imagenHandler;
    private const string TOPIC_IMAGEN_MODALIDAD = "imagen-modalidad";
    private const string SUBSCRIPTION_NAME = "modalidad-service";

    public ImagenModalidadSubscriptionWorker(
        ILogger<ImagenModalidadSubscriptionWorker> logger,
        IMessageConsumer messageConsumer,
        ImagenModalidadHandler imagenHandler)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
        _imagenHandler = imagenHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_IMAGEN_MODALIDAD, SUBSCRIPTION_NAME);

            await _messageConsumer.StartAsync<ImagenModalidadEvent>(
                TOPIC_IMAGEN_MODALIDAD,
                SUBSCRIPTION_NAME,
                async (evento) => await _imagenHandler.HandleImagenModalidad(evento)
            );

            // Keep the worker running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in subscription worker");
            throw;
        }
        finally
        {
            await _messageConsumer.StopAsync();
        }
    }
}
