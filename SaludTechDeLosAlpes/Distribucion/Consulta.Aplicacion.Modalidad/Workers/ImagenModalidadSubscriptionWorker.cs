using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Modalidad.Comandos;
using Consulta.Dominio.Eventos;
using Microsoft.Extensions.DependencyInjection;

namespace Consulta.Aplicacion.Modalidad.Workers;

public class ImagenModalidadSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenModalidadSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_IMAGEN_MODALIDAD = "imagen-modalidad";
    private const string SUBSCRIPTION_NAME = "modalidad-service";

    public ImagenModalidadSubscriptionWorker(
        ILogger<ImagenModalidadSubscriptionWorker> logger,
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
            var imagenHandler = scope.ServiceProvider.GetRequiredService<ImagenModalidadHandler>();

            try
            {
                _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                    TOPIC_IMAGEN_MODALIDAD, SUBSCRIPTION_NAME);

                await messageConsumer.StartAsync<ImagenModalidadEvent>(
                    TOPIC_IMAGEN_MODALIDAD,
                    SUBSCRIPTION_NAME,
                    imagenHandler.HandleImagenModalidad
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