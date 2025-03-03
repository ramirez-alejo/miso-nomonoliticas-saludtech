using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Anonimizacion.Comandos;
using Ingestion.Aplicacion.Anonimizacion.Modelos;

namespace Ingestion.Aplicacion.Anonimizacion.Workers;

public class AnonimizarImagenSubscriptionWorker : BackgroundService
{
    private readonly ILogger<AnonimizarImagenSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_IMAGEN_ANONIMIZAR = "imagen-anonimizar";
    private const string SUBSCRIPTION_NAME = "anonimizar-service";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;

    public AnonimizarImagenSubscriptionWorker(
        ILogger<AnonimizarImagenSubscriptionWorker> logger,
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
            var anonimizarHandler = _scope.Value.ServiceProvider.GetRequiredService<AnonimizarHandler>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_IMAGEN_ANONIMIZAR, SUBSCRIPTION_NAME);

            await _messageConsumer.StartWithSchemaAsync<Ingestion.Dominio.Comandos.Anonimizar>(
                TOPIC_IMAGEN_ANONIMIZAR,
                SUBSCRIPTION_NAME,
                anonimizarHandler.HandleAnonimizar
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
