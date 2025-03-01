using Consulta.Aplicacion.Sagas;
using Consulta.Dominio.Eventos;
using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulta.Aplicacion.Workers;

public class ImagenConsultaResponseWorker : BackgroundService
{
    private readonly ILogger<ImagenConsultaResponseWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_DEMOGRAFIA_RESPONSE = "imagen-consulta-demografia-response";
    private const string TOPIC_MODALIDAD_RESPONSE = "imagen-consulta-modalidad-response";
    private const string TOPIC_DATA_RESPONSE = "imagen-consulta-data-response";
    private const string SUBSCRIPTION_NAME_DEMOGRAFIA = "demografia-response-handler";
    private const string SUBSCRIPTION_NAME_MODALIDAD = "modalidad-response-handler";
    private const string SUBSCRIPTION_NAME_DATA = "data-response-handler";
    private IMessageConsumer? _demografiaConsumer;
    private IMessageConsumer? _modalidadConsumer;
    private IMessageConsumer? _dataConsumer;
    private AsyncServiceScope? _scope;
    private AsyncServiceScope? _modalidadScope;
    private AsyncServiceScope? _dataScope;

    public ImagenConsultaResponseWorker(
        ILogger<ImagenConsultaResponseWorker> logger,
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
            var orchestrator = _scope.Value.ServiceProvider.GetRequiredService<ImagenConsultaSagaOrchestrator>();
            
            // Get the message consumers from the DI container
            _demografiaConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();
            
            // Start demografia response consumer
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_DEMOGRAFIA_RESPONSE, SUBSCRIPTION_NAME_DEMOGRAFIA);
            await _demografiaConsumer.StartAsync<ImagenConsultaDemografiaResponseEvent>(
                TOPIC_DEMOGRAFIA_RESPONSE,
                SUBSCRIPTION_NAME_DEMOGRAFIA,
                async response => 
                {
                    _logger.LogInformation("Received demografia response for saga {SagaId}", response.SagaId);
                    await orchestrator.HandleDemografiaResponse(response);
                }
            );
            
            // Create a new scope for the modalidad consumer
            _modalidadScope = _serviceProvider.CreateAsyncScope();
            _modalidadConsumer = _modalidadScope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();
            
            // Create a new scope for the data consumer
            _dataScope = _serviceProvider.CreateAsyncScope();
            _dataConsumer = _dataScope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();

            // Start modalidad response consumer
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_MODALIDAD_RESPONSE, SUBSCRIPTION_NAME_MODALIDAD);
            await _modalidadConsumer.StartAsync<ImagenConsultaModalidadResponseEvent>(
                TOPIC_MODALIDAD_RESPONSE,
                SUBSCRIPTION_NAME_MODALIDAD,
                async response => 
                {
                    _logger.LogInformation("Received modalidad response for saga {SagaId}", response.SagaId);
                    await orchestrator.HandleModalidadResponse(response);
                }
            );

            // Start data response consumer
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_DATA_RESPONSE, SUBSCRIPTION_NAME_DATA);
            await _dataConsumer.StartAsync<ImagenConsultaDataResponseEvent>(
                TOPIC_DATA_RESPONSE,
                SUBSCRIPTION_NAME_DATA,
                async response => 
                {
                    _logger.LogInformation("Received data warehouse response for saga {SagaId}", response.SagaId);
                    await orchestrator.HandleDataResponse(response);
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
            _logger.LogError(ex, "Error in response worker");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        try
        {
            await base.StopAsync(stoppingToken);
            
            // Stop and dispose all consumers
            if (_demografiaConsumer != null)
            {
                try 
                {
                    await _demografiaConsumer.StopAsync();
                    if (_demografiaConsumer is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping demografia consumer");
                }
            }

            if (_modalidadConsumer != null)
            {
                try 
                {
                    await _modalidadConsumer.StopAsync();
                    if (_modalidadConsumer is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping modalidad consumer");
                }
            }

            if (_dataConsumer != null)
            {
                try 
                {
                    await _dataConsumer.StopAsync();
                    if (_dataConsumer is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping data consumer");
                }
            }

            if (_scope.HasValue)
            {
                await _scope.Value.DisposeAsync();
            }
            
            if (_modalidadScope.HasValue)
            {
                await _modalidadScope.Value.DisposeAsync();
            }
            
            if (_dataScope.HasValue)
            {
                await _dataScope.Value.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during worker shutdown");
        }
    }
}
