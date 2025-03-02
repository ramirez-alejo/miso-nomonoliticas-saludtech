using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Events;
using Ingestion.Infraestructura.Persistencia;
using Ingestion.Infraestructura.Persistencia.Repositorios;

namespace Ingestion.Aplicacion.Workers;

public class ImagenConsultaDataWarehouseRequestWorker : BackgroundService
{
    private readonly ILogger<ImagenConsultaDataWarehouseRequestWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_DATAWAREHOUSE_REQUEST = "imagen-consulta-datawarehouse-request";
    private const string TOPIC_DATA_RESPONSE = "imagen-consulta-data-response";
    private const string SUBSCRIPTION_NAME = "datawarehouse-request-handler";
    private IMessageConsumer? _messageConsumer;
    private IMessageProducer? _messageProducer;
    private AsyncServiceScope? _scope;
    private IImagenRepository _imagenRepository;

    public ImagenConsultaDataWarehouseRequestWorker(
        ILogger<ImagenConsultaDataWarehouseRequestWorker> logger,
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
            _messageProducer = _scope.Value.ServiceProvider.GetRequiredService<IMessageProducer>();
            _imagenRepository = _scope.Value.ServiceProvider.GetRequiredService<IImagenRepository>();
            
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_DATAWAREHOUSE_REQUEST, SUBSCRIPTION_NAME);

            await _messageConsumer.StartAsync<ImagenConsultaDataWarehouseRequestEvent>(
                TOPIC_DATAWAREHOUSE_REQUEST,
                SUBSCRIPTION_NAME,
                async request => 
                {
                    _logger.LogInformation("Received data warehouse request for saga {SagaId}", request.SagaId);
                    await HandleDataWarehouseRequest(request, stoppingToken);
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

    private async Task HandleDataWarehouseRequest(ImagenConsultaDataWarehouseRequestEvent request, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // TODO: Mover al repositorio
            // Query the database for the requested images
            var imagenes = await _imagenRepository.GetByIdsAsync(request.ImagenIds, cancellationToken);

            // Publish response event
            await _messageProducer.SendJsonAsync(TOPIC_DATA_RESPONSE, new ImagenConsultaDataResponseEvent
            {
                SagaId = request.SagaId,
                Imagenes = imagenes,
                Success = true
            });

            _logger.LogInformation("Data warehouse request completed for saga {SagaId}, found {Count} images", 
                request.SagaId, imagenes.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in data warehouse request for saga {SagaId}: {Message}", request.SagaId, ex.Message);
            
            // Publish failure event
            await _messageProducer.SendJsonAsync(TOPIC_DATA_RESPONSE, new ImagenConsultaDataResponseEvent
            {
                SagaId = request.SagaId,
                Imagenes = Array.Empty<Imagen>(),
                Success = false,
                ErrorMessage = ex.Message
            });
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
