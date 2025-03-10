using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Sagas;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Workers;

public class MetadataGeneradaSubscriber : BackgroundService
{
    private readonly ILogger<MetadataGeneradaSubscriber> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_METADATA_GENERADA = "imagen-metadata-generada";
    private const string SUBSCRIPTION_NAME = "ingestion-saga-metadata";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;
    private ImagenIngestionSagaOrchestrator? _orchestrator;

    public MetadataGeneradaSubscriber(
        ILogger<MetadataGeneradaSubscriber> logger,
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
            _orchestrator = _scope.Value.ServiceProvider.GetRequiredService<ImagenIngestionSagaOrchestrator>();
            _messageConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}", 
                TOPIC_METADATA_GENERADA, SUBSCRIPTION_NAME);

            // Subscribe to MetadataGenerada events
            await _messageConsumer.StartAsync<MetadataGenerada>(
                TOPIC_METADATA_GENERADA,
                SUBSCRIPTION_NAME,
                async (evento) =>
                {
                    _logger.LogInformation("Received metadata generada event for saga {SagaId}", evento.SagaId);
                    await _orchestrator!.HandleMetadataGeneradaEvent(evento);
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
            _logger.LogError(ex, "Error in MetadataGeneradaSubscriber");
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
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping metadata consumer");
                }
            }

            if (_scope.HasValue)
            {
                await _scope.Value.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during MetadataGeneradaSubscriber shutdown");
        }
    }
}
