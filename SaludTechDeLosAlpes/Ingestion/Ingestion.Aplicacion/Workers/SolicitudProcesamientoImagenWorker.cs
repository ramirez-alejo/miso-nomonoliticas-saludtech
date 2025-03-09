using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Comandos;
using Ingestion.Aplicacion.Dtos;
using Ingestion.Dominio.Eventos;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Dominio;
using Ingestion.Aplicacion.Mapeo;

namespace Ingestion.Aplicacion.Workers;

/// <summary>
/// Worker that subscribes to SolicitudProcesamientoImagen events from the BFF
/// </summary>
public class SolicitudProcesamientoImagenWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SolicitudProcesamientoImagenWorker> _logger;
    
    // Topic constants
    private const string TOPIC_SOLICITUD_PROCESAMIENTO = "bff-solicitud-procesamiento";
    private const string SUBSCRIPTION_NAME = "ingestion-solicitud-procesamiento-subscription";
    
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;
    
    public SolicitudProcesamientoImagenWorker(
        IServiceProvider serviceProvider,
        ILogger<SolicitudProcesamientoImagenWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Iniciando SolicitudProcesamientoImagenWorker");
            
            // Create a scope to resolve dependencies
            _scope = _serviceProvider.CreateAsyncScope();
            var mediator = _scope.Value.ServiceProvider.GetRequiredService<IMediator>();
            _messageConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();
            
            await _messageConsumer.StartWithSchemaAsync<SolicitudProcesamientoImagen>(
                TOPIC_SOLICITUD_PROCESAMIENTO,
                SUBSCRIPTION_NAME,
                async (solicitud) => await HandleSolicitudProcesamientoAsync(solicitud, mediator));
            
            _logger.LogInformation("SolicitudProcesamientoImagenWorker iniciado y suscrito a {Topic}", TOPIC_SOLICITUD_PROCESAMIENTO);
            
            // Keep the worker running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en SolicitudProcesamientoImagenWorker");
            throw;
        }
    }
    
    private async Task HandleSolicitudProcesamientoAsync(SolicitudProcesamientoImagen solicitud, IMediator mediator)
    {
        try
        {
            // Map the SolicitudProcesamientoImagen to ImagenDto
            var imagenDto = MapToImagenDto(solicitud);
            
            // map the SolicitudProcesamientoImagen to CrearImagenMedicaCommand (since both inherit from Imagen)
            var imagenCommand = MapeoImagen.MapToCommand(solicitud);
            
            _logger.LogInformation("Creando imagen: {@ImagenId}", imagenCommand);
            
            var imagen = await mediator.Send(imagenCommand);
            imagenDto.Id = imagen.Id;
            
            _logger.LogInformation("Recibida solicitud de procesamiento: CorrelationId={CorrelationId}, ImagenId={ImagenId}",
                solicitud.CorrelationId, solicitud.Id);
            
            // Start the saga with the correlation ID
            var command = new StartImagenIngestionSagaCommand(imagenDto, solicitud.CorrelationId);
            var sagaId = await mediator.Send(command);
            
            _logger.LogInformation("Iniciada saga {SagaId} para solicitud de procesamiento con CorrelationId={CorrelationId}",
                sagaId, solicitud.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar solicitud de procesamiento: {CorrelationId}", solicitud.CorrelationId);
        }
    }
    
    private ImagenDto MapToImagenDto(SolicitudProcesamientoImagen solicitud)
    {
        return new ImagenDto
        {
            Id = solicitud.Id,
            Nombre = $"Imagen {solicitud.Id}",
            Descripcion = $"Modalidad: {solicitud.TipoImagen?.Modalidad?.Nombre}, Región: {solicitud.TipoImagen?.RegionAnatomica?.Nombre}",
            FechaCreacion = DateTime.UtcNow,
            Url = $"https://datawarehouse.saludtech.com/imagenes/{solicitud.Id}.jpg"
        };
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
