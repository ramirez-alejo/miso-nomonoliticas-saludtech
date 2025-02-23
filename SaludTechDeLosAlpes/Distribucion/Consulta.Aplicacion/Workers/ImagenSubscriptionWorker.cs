using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulta.Aplicacion.Workers;

public class ImagenSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenSubscriptionWorker> _logger;
    private readonly IMessageConsumer _messageConsumer;
    private const string TOPIC_IMAGEN_MEDICA = "imagen-medica";
    private const string SUBSCRIPTION_NAME = "consulta-service";

    public ImagenSubscriptionWorker(
        ILogger<ImagenSubscriptionWorker> logger,
        IMessageConsumer messageConsumer)
    {
        _logger = logger;
        _messageConsumer = messageConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_IMAGEN_MEDICA, SUBSCRIPTION_NAME);

            await _messageConsumer.StartAsync<Imagen>(
                TOPIC_IMAGEN_MEDICA,
                SUBSCRIPTION_NAME,
                async (imagen) => await Task.CompletedTask // actual handling is done in ImagenCreadaHandler
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
