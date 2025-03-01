using Consulta.Aplicacion.Sagas;
using Consulta.Aplicacion.Sagas.Events;
using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulta.Aplicacion.Workers;

public class ImagenConsultaDataWorker : BackgroundService
{
    private readonly ILogger<ImagenConsultaDataWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_DATA_REQUEST = "imagen-consulta-data-request";
    private const string SUBSCRIPTION_NAME = "data-request-handler";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;

    public ImagenConsultaDataWorker(
        ILogger<ImagenConsultaDataWorker> logger,
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
            var orchestrator = _scope.Value.ServiceProvider.GetRequiredService<ImagenConsultaSagaOrchestrator>();
            
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_DATA_REQUEST, SUBSCRIPTION_NAME);

            await _messageConsumer.StartAsync<ImagenConsultaDataRequestEvent>(
                TOPIC_DATA_REQUEST,
                SUBSCRIPTION_NAME,
                async request => 
                {
                    _logger.LogInformation("Received data warehouse request for saga {SagaId}", request.SagaId);
                    await orchestrator.HandleDataRequest(request.SagaId, request.ImagenIds);
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
            _logger.LogError(ex, "Error in data warehouse request worker");
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
