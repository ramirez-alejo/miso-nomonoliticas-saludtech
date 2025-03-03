using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Metadata.Handlers;

namespace Ingestion.Aplicacion.Metadata.Workers;

public class MetadataImagenSubscriptionWorker: BackgroundService
{
    private readonly ILogger<MetadataImagenSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_IMAGENT_METADATA = "metadata-imagen";
    private const string SUBSCRIPTION_NAME = "metadata-service";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;

    public MetadataImagenSubscriptionWorker(
        ILogger<MetadataImagenSubscriptionWorker> logger,
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
            var handler = _scope.Value.ServiceProvider.GetRequiredService<GenerarMetadataHandler>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_IMAGENT_METADATA, SUBSCRIPTION_NAME);

            await _messageConsumer.StartWithSchemaAsync<Ingestion.Dominio.Comandos.GenerarMetadata>(
                TOPIC_IMAGENT_METADATA,
                SUBSCRIPTION_NAME,
                handler.HandleGenerarMetadata
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
