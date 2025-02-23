using System.Text.Json;
using Consulta.Aplicacion.Demografia.Comandos;
using Consulta.Dominio.Eventos;
using Core.Infraestructura.MessageBroker;

namespace Consulta.Aplicacion.Demografia.Workers;

public class ImagenDemograficaSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenDemograficaSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_DEMOGRAFIA = "imagen-demografia";
    private const string SUBSCRIPTION_NAME = "demografia-service";

    public ImagenDemograficaSubscriptionWorker(
        ILogger<ImagenDemograficaSubscriptionWorker> logger,
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
            var imagenHandler = scope.ServiceProvider.GetRequiredService<ImagenDemografiaHandler>();

            try
            {
                _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                    TOPIC_DEMOGRAFIA, SUBSCRIPTION_NAME);

                await messageConsumer.StartAsync<ImagenDemografiaEvent>(
                    TOPIC_DEMOGRAFIA,
                    SUBSCRIPTION_NAME,
                    imagenHandler.HandleImagenDemografia
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
