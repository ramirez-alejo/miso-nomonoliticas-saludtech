using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Modalidad.Comandos;
using Consulta.Dominio.Eventos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulta.Aplicacion.Modalidad.Workers;

public class ImagenModalidadSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenModalidadSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_IMAGEN_MODALIDAD = "imagen-modalidad";
    private const string SUBSCRIPTION_NAME = "modalidad-service";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;

    public ImagenModalidadSubscriptionWorker(
        ILogger<ImagenModalidadSubscriptionWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _scope = _serviceProvider.CreateAsyncScope();
            _messageConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var imagenHandler = _scope.Value.ServiceProvider.GetRequiredService<ImagenModalidadHandler>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_IMAGEN_MODALIDAD, SUBSCRIPTION_NAME);

            await _messageConsumer.StartWithSchemaAsync<ImagenModalidadEvent>(
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
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {

        try
        {
            await base.StopAsync(stoppingToken);
            if (_messageConsumer != null)
            {
                try 
                {
                    await _messageConsumer.StopAsync();
                    if (_messageConsumer is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping message consumer");
                }
            }

            if (_scope.HasValue)
            {
                await _scope.Value.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during worker shutdown");
        }
    }
}
