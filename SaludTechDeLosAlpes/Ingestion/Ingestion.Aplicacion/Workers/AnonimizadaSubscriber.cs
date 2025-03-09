using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Sagas;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Workers;

public class AnonimizadaSubscriber : BackgroundService
{
    private readonly ILogger<AnonimizadaSubscriber> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_ANONIMIZADA = "imagen-anonimizada";
    private const string SUBSCRIPTION_NAME = "ingestion-saga-anonimizada";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;
    private ImagenIngestionSagaOrchestrator? _orchestrator;

    public AnonimizadaSubscriber(
        ILogger<AnonimizadaSubscriber> logger,
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
                TOPIC_ANONIMIZADA, SUBSCRIPTION_NAME);

            // Subscribe to Anonimizada events
            await _messageConsumer.StartAsync<Anonimizada>(
                TOPIC_ANONIMIZADA,
                SUBSCRIPTION_NAME,
                async (evento) =>
                {
                    _logger.LogInformation("Received anonimizada event for saga {SagaId}", evento.SagaId);
                    await _orchestrator!.HandleAnonimizadaEvent(evento);
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
            _logger.LogError(ex, "Error in AnonimizadaSubscriber");
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
                    _logger.LogError(ex, "Error stopping anonimizada consumer");
                }
            }

            if (_scope.HasValue)
            {
                await _scope.Value.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during AnonimizadaSubscriber shutdown");
        }
    }
}
