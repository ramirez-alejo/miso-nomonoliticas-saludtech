using Consulta.Aplicacion.Demografia.Consultas;
using Consulta.Aplicacion.Demografia.Persistencia.Repositorios;
using Consulta.Aplicacion.Demografia.Sagas.Events;
using Core.Infraestructura.MessageBroker;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulta.Aplicacion.Demografia.Workers;

public class ImagenConsultaDemografiaRequestWorker : BackgroundService
{
    private readonly ILogger<ImagenConsultaDemografiaRequestWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_DEMOGRAFIA_REQUEST = "imagen-consulta-demografia-request";
    private const string TOPIC_DEMOGRAFIA_RESPONSE = "imagen-consulta-demografia-response";
    private const string SUBSCRIPTION_NAME = "demografia-request-handler";
    private IMessageConsumer? _messageConsumer;
    private IMessageProducer? _messageProducer;
    private AsyncServiceScope? _scope;

    public ImagenConsultaDemografiaRequestWorker(
        ILogger<ImagenConsultaDemografiaRequestWorker> logger,
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
            var repository = _scope.Value.ServiceProvider.GetRequiredService<IImagenDemografiaRepository>();
            
            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_DEMOGRAFIA_REQUEST, SUBSCRIPTION_NAME);

            await _messageConsumer.StartAsync<ImagenConsultaDemografiaRequestEvent>(
                TOPIC_DEMOGRAFIA_REQUEST,
                SUBSCRIPTION_NAME,
                async request => 
                {
                    try
                    {
                        _logger.LogInformation("Received demografia request for saga {SagaId}", request.SagaId);
                        
                        // Create a query from the request
                        var query = new ImagenDemografiaConsulta
                        {
                            GrupoEdad = request.Filter.GrupoEdad,
                            Sexo = request.Filter.Sexo,
                            Etnicidad = request.Filter.Etnicidad
                        };
                        
                        // Process the query directly using the mediator
                        var result = await mediator.Send(query, stoppingToken);
                        
                        // Publish the response event
                        await _messageProducer.SendJsonAsync(TOPIC_DEMOGRAFIA_RESPONSE, new ImagenConsultaDemografiaResponseEvent
                        {
                            SagaId = request.SagaId,
                            ImagenIds = result.ImagenIds.ToArray(),
                            Success = true
                        });
                        
                        _logger.LogInformation("Processed demografia request for saga {SagaId}, found {Count} matching images", 
                            request.SagaId, result.ImagenIds.Count());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing demografia request for saga {SagaId}", request.SagaId);
                        
                        // Publish failure response
                        await _messageProducer.SendJsonAsync(TOPIC_DEMOGRAFIA_RESPONSE, new ImagenConsultaDemografiaResponseEvent
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
            _logger.LogError(ex, "Error in demografia request worker");
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
