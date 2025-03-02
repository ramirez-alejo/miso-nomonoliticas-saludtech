using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Dtos;
using Ingestion.Aplicacion.Events;
using Ingestion.Aplicacion.Mapeo;
using Ingestion.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

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
    private HttpClient? _httpClient;
    private IConfiguration? _configuration;
    private AsyncServiceScope? _scope;

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
            _httpClient = _scope.Value.ServiceProvider.GetRequiredService<HttpClient>();
            _configuration = _scope.Value.ServiceProvider.GetRequiredService<IConfiguration>();
            
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
            var dbContext = scope.ServiceProvider.GetRequiredService<ImagenDbContext>();
            
            // TODO: Mover al repositorio
            // Query the database for the requested images
            var imagenEntities = await dbContext.Imagenes
                .Include(i => i.TipoImagen)
                    .ThenInclude(t => t.Modalidad)
                .Include(i => i.TipoImagen)
                    .ThenInclude(t => t.RegionAnatomica)
                .Include(i => i.TipoImagen)
                    .ThenInclude(t => t.Patologia)
                .Include(i => i.AtributosImagen)
                .Include(i => i.ContextoProcesal)
                .Include(i => i.Metadatos)
                    .ThenInclude(m => m.EntornoClinico)
                .Include(i => i.Metadatos)
                    .ThenInclude(m => m.Sintomas)
                .Include(i => i.Paciente)
                    .ThenInclude(p => p.Demografia)
                .Include(i => i.Paciente)
                    .ThenInclude(p => p.Historial)
                .Where(i => request.ImagenIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            // Map the entities to the core domain model
            var imagenes = imagenEntities.Select(MapeoImagen.MapToCore).ToArray();

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
