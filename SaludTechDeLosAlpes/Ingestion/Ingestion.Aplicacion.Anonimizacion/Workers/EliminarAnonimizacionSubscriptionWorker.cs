using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Anonimizacion.Comandos;

namespace Ingestion.Aplicacion.Anonimizacion.Workers;

public class EliminarAnonimizacionSubscriptionWorker : BackgroundService
{
    private readonly ILogger<EliminarAnonimizacionSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_IMAGEN_ELIMINAR_ANONIMIZACION = "imagen-eliminar-anonimizacion";
    private const string SUBSCRIPTION_NAME = "eliminar-anonimizacion-service";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;

    public EliminarAnonimizacionSubscriptionWorker(
        ILogger<EliminarAnonimizacionSubscriptionWorker> logger,
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
            var eliminarAnonimizacionHandler = _scope.Value.ServiceProvider.GetRequiredService<EliminarAnonimizacionHandler>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_IMAGEN_ELIMINAR_ANONIMIZACION, SUBSCRIPTION_NAME);

            await _messageConsumer.StartWithSchemaAsync<Ingestion.Dominio.Comandos.EliminarAnonimizacion>(
                TOPIC_IMAGEN_ELIMINAR_ANONIMIZACION,
                SUBSCRIPTION_NAME,
                eliminarAnonimizacionHandler.HandleEliminarAnonimizacion
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
