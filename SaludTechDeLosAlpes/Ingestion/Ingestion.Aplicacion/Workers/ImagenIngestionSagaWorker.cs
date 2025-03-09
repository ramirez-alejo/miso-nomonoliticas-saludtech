using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Sagas;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Workers;

/// <summary>
/// OBSOLETE: This worker has been replaced by AnonimizadaSubscriber and MetadataGeneradaSubscriber.
/// These new subscribers handle each event type separately and inject the saga orchestrator.
/// </summary>
[Obsolete("This worker has been replaced by AnonimizadaSubscriber and MetadataGeneradaSubscriber")]
public class ImagenIngestionSagaWorker : BackgroundService
{
    private readonly ILogger<ImagenIngestionSagaWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_ANONIMIZADA = "imagen-anonimizada";
    private const string TOPIC_METADATA_GENERADA = "imagen-metadata-generada";
    private const string SUBSCRIPTION_NAME_ANONIMIZADA = "ingestion-saga-anonimizada";
    private const string SUBSCRIPTION_NAME_METADATA = "ingestion-saga-metadata";
    private IMessageConsumer? _anonimizadaConsumer;
    private IMessageConsumer? _metadataConsumer;
    private AsyncServiceScope? _scope;
    private AsyncServiceScope? _metadataScope;

    public ImagenIngestionSagaWorker(
        ILogger<ImagenIngestionSagaWorker> logger,
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
            var orchestrator = _scope.Value.ServiceProvider.GetRequiredService<ImagenIngestionSagaOrchestrator>();
            
            // Get the message consumer for anonimizada events
            _anonimizadaConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_ANONIMIZADA, SUBSCRIPTION_NAME_ANONIMIZADA);

            // Subscribe to Anonimizada events
            await _anonimizadaConsumer.StartAsync<Anonimizada>(
                TOPIC_ANONIMIZADA,
                SUBSCRIPTION_NAME_ANONIMIZADA,
                async (evento) =>
                {
                    _logger.LogInformation("Received anonimizada event for saga {SagaId}", evento.SagaId);
                    await orchestrator.HandleAnonimizadaEvent(evento);
                }
            );

            // Create a new scope for the metadata consumer
            _metadataScope = _serviceProvider.CreateAsyncScope();
            _metadataConsumer = _metadataScope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_METADATA_GENERADA, SUBSCRIPTION_NAME_METADATA);

            // Subscribe to MetadataGenerada events
            await _metadataConsumer.StartAsync<MetadataGenerada>(
                TOPIC_METADATA_GENERADA,
                SUBSCRIPTION_NAME_METADATA,
                async (evento) =>
                {
                    _logger.LogInformation("Received metadata generada event for saga {SagaId}", evento.SagaId);
                    await orchestrator.HandleMetadataGeneradaEvent(evento);
                }
            );

            // Keep the worker running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in saga worker");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        try
        {
            await base.StopAsync(stoppingToken);
            
            // Stop anonimizada consumer
            if (_anonimizadaConsumer != null)
            {
                try 
                {
                    await _anonimizadaConsumer.StopAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping anonimizada consumer");
                }
            }

            // Stop metadata consumer
            if (_metadataConsumer != null)
            {
                try 
                {
                    await _metadataConsumer.StopAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping metadata consumer");
                }
            }

            // Dispose scopes
            if (_scope.HasValue)
            {
                await _scope.Value.DisposeAsync();
            }
            
            if (_metadataScope.HasValue)
            {
                await _metadataScope.Value.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during worker shutdown");
        }
    }
}
