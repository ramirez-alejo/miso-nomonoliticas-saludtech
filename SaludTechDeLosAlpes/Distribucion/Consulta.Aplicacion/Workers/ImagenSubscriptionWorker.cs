using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Consulta.Aplicacion.Comandos;

namespace Consulta.Aplicacion.Workers;

public class ImagenSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_IMAGEN_MEDICA = "imagen-medica";
    private const string SUBSCRIPTION_NAME = "consulta-service";

    public ImagenSubscriptionWorker(
        ILogger<ImagenSubscriptionWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var messageConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var imagenHandler = scope.ServiceProvider.GetRequiredService<ImagenCreadaHandler>();
            try
            {
                _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                    TOPIC_IMAGEN_MEDICA, SUBSCRIPTION_NAME);

                await messageConsumer.StartAsync<Imagen>(
                    TOPIC_IMAGEN_MEDICA,
                    SUBSCRIPTION_NAME,
                    async imagen => await imagenHandler.HandleImagenCreada(imagen)
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
                await messageConsumer.StopAsync();
            }
        }
    }
}
