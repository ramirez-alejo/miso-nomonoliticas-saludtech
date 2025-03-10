using BFF.API.Services;
using Core.Infraestructura.MessageBroker;
using Ingestion.Dominio.Eventos;
using Microsoft.Extensions.DependencyInjection;

namespace BFF.API.Workers;

/// <summary>
/// Background worker that subscribes to SagaIniciada events
/// </summary>
public class SagaStatusSubscriptionWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SagaStatusSubscriptionWorker> _logger;
    
    // Topic constants
    private const string TOPIC_SAGA_STATUS = "saga-status";
    private const string SUBSCRIPTION_NAME = "bff-saga-iniciada-subscription";
    
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;
    
    public SagaStatusSubscriptionWorker(
        IServiceProvider serviceProvider,
        ILogger<SagaStatusSubscriptionWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Iniciando SagaIniciadaSubscriptionWorker");
            
            // Create a scope to resolve dependencies
            _scope = _serviceProvider.CreateAsyncScope();
            var imagenService = _scope.Value.ServiceProvider.GetRequiredService<IImagenService>();
            _messageConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();
            
            await _messageConsumer.StartWithSchemaAsync<EstadoSaga>(
                TOPIC_SAGA_STATUS,
                SUBSCRIPTION_NAME,
                async (sagaIniciada) => await HandleSagaStatusEventAsync(sagaIniciada, imagenService));
            
            _logger.LogInformation("SagaIniciadaSubscriptionWorker iniciado y suscrito a {Topic}", TOPIC_SAGA_STATUS);
            
            // Keep the worker running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en SagaIniciadaSubscriptionWorker");
            throw;
        }
    }
    
    private async Task HandleSagaStatusEventAsync(EstadoSaga estadoSaga, IImagenService imagenService)
    {
        try
        {
            _logger.LogInformation("Recibido evento SagaIniciada: CorrelationId={CorrelationId}, SagaId={SagaId}, ImagenId={ImagenId}",
                estadoSaga.CorrelationId, estadoSaga.SagaId, estadoSaga.ImagenId);


            await imagenService.HandleSagaStatusAsync(
                estadoSaga.CorrelationId,
                estadoSaga.SagaId,
                estadoSaga.ImagenId,
                estadoSaga.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar evento SagaIniciada: {CorrelationId}", estadoSaga.CorrelationId);
        }
    }
    
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        try
        {
            await base.StopAsync(stoppingToken);
            
            // Stop message consumer
            if (_messageConsumer != null)
            {
                try 
                {
                    await _messageConsumer.StopAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping message consumer");
                }
            }

            // Dispose scope
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
