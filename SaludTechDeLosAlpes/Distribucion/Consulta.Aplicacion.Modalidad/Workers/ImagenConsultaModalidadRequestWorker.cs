using Consulta.Aplicacion.Modalidad.Consultas;
using Consulta.Dominio.Eventos;
using Core.Infraestructura.MessageBroker;
using Mediator;

namespace Consulta.Aplicacion.Modalidad.Workers;

public class ImagenConsultaModalidadRequestWorker : BackgroundService
{
    private readonly ILogger<ImagenConsultaModalidadRequestWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_MODALIDAD_REQUEST = "imagen-consulta-modalidad-request";
    private const string TOPIC_MODALIDAD_RESPONSE = "imagen-consulta-modalidad-response";
    private const string SUBSCRIPTION_NAME = "modalidad-request-handler";
    private IMessageConsumer? _messageConsumer;
    private IMessageProducer? _messageProducer;
    private AsyncServiceScope? _scope;

    public ImagenConsultaModalidadRequestWorker(
        ILogger<ImagenConsultaModalidadRequestWorker> logger,
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
            var mediator = _scope.Value.ServiceProvider.GetRequiredService<IMediator>();
            
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_MODALIDAD_REQUEST, SUBSCRIPTION_NAME);

            await _messageConsumer.StartAsync<ImagenConsultaModalidadRequestCommand>(
                TOPIC_MODALIDAD_REQUEST,
                SUBSCRIPTION_NAME,
                async request => 
                {
                    try
                    {
                        _logger.LogInformation("Received modalidad request for saga {SagaId}", request.SagaId);
                        
                        // Create a query from the request
                        var query = new ImagenTipoImagenConsulta
                        {
                            Modalidad = request.Filter.Modalidad,
                            RegionAnatomica = request.Filter.RegionAnatomica
                        };
                        
                        // Process the query directly using the mediator
                        var result = await mediator.Send(query, stoppingToken);
                        
                        // Publish the response event
                        await _messageProducer.SendJsonAsync(TOPIC_MODALIDAD_RESPONSE, new ImagenConsultaModalidadResponseEvent
                        {
                            SagaId = request.SagaId,
                            ImagenIds = result.ImagenId.ToArray(),
                            Success = true
                        });
                        
                        _logger.LogInformation("Processed modalidad request for saga {SagaId}, found {Count} matching images", 
                            request.SagaId, result.ImagenId.Count());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing modalidad request for saga {SagaId}", request.SagaId);
                        
                        // Publish failure response
                        await _messageProducer.SendJsonAsync(TOPIC_MODALIDAD_RESPONSE, new ImagenConsultaModalidadResponseEvent
                        {
                            SagaId = request.SagaId,
                            ImagenIds = Array.Empty<Guid>(),
                            Success = false,
                            ErrorMessage = ex.Message
                        });
                    }
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
            _logger.LogError(ex, "Error in modalidad request worker");
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
