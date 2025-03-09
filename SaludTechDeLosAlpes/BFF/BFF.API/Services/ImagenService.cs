using System.Text.Json;
using BFF.API.Controllers;
using BFF.API.Persistence;
using Core.Infraestructura.MessageBroker;
using Ingestion.Dominio.Eventos;
using Ingestion.Dominio.Saga;

namespace BFF.API.Services;

/// <summary>
/// Implementation of the imagen service
/// </summary>
public class ImagenService : IImagenService
{
    private readonly ICorrelationRepository _correlationRepository;
    private readonly IMessageProducer _messageProducer;
    private readonly ILogger<ImagenService> _logger;
    
    // Topic constants
    private const string TOPIC_SOLICITUD_PROCESAMIENTO = "bff-solicitud-procesamiento";
    private const int DEFAULT_TTL_SECONDS = 86400; // 24 hours
    
    public ImagenService(
        ICorrelationRepository correlationRepository,
        IMessageProducer messageProducer,
        ILogger<ImagenService> logger)
    {
        _correlationRepository = correlationRepository;
        _messageProducer = messageProducer;
        _logger = logger;
    }
    
    /// <inheritdoc />
    public async Task<Guid> IniciarProcesamientoAsync(SolicitudProcesamientoRequest imagen, string descripcion = null)
    {
        // Generate a new correlation ID
        var correlationId = Guid.NewGuid();
        
        _logger.LogInformation("Iniciando procesamiento para imagen con correlationId {CorrelationId}", 
            correlationId);
        
        // Store the correlation info in Redis without the saga ID initially
        var correlationInfo = new CorrelationInfo
        {
            CorrelationId = correlationId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _correlationRepository.SaveCorrelationAsync(correlationInfo, DEFAULT_TTL_SECONDS);
        
        // Create and publish the event to request image processing
        var solicitud = new SolicitudProcesamientoImagen
        {
            CorrelationId = correlationId,
            Version = imagen.Version,
            TipoImagen = imagen.TipoImagen,
            AtributosImagen = imagen.AtributosImagen,
            ContextoProcesal = imagen.ContextoProcesal,
            Metadatos = imagen.Metadatos,
            Paciente = imagen.Paciente
        };
        
        await _messageProducer.SendWithSchemaAsync(TOPIC_SOLICITUD_PROCESAMIENTO, solicitud);
        
        //_logger.LogInformation("Solicitud de procesamiento publicada para imagen con correlationId {CorrelationId} , imagen {Solicitud}", correlationId, JsonSerializer.Serialize(solicitud));
        
        return correlationId;
    }
    
    /// <inheritdoc />
    public async Task<CorrelationInfo> ObtenerEstadoProcesamientoAsync(Guid correlationId)
    {
        _logger.LogInformation("Obteniendo estado de procesamiento para correlationId {CorrelationId}", correlationId);
        
        var correlationInfo = await _correlationRepository.GetCorrelationAsync(correlationId);
        
        if (correlationInfo == null)
        {
            _logger.LogWarning("No se encontró información para correlationId {CorrelationId}", correlationId);
            return null;
        }
        
        return correlationInfo;
    }

    public async Task<ImagenIngestionSagaState> ObtenerEstadoSagaAsync(Guid sagaId)
    {
        _logger.LogInformation("Obteniendo estado de saga para sagaId {SagaId}", sagaId);
        
        var sagaState = await _correlationRepository.GetSagaStateAsync(sagaId);
        
        if (sagaState == null)
        {
            _logger.LogWarning("No se encontró información para sagaId {SagaId}", sagaId);
            return null;
        }
        
        return sagaState;
    }
    
    /// <inheritdoc />
    public async Task HandleSagaIniciadaAsync(Guid correlationId, Guid sagaId, Guid imagenId)
    {
        _logger.LogInformation("Manejando evento SagaIniciada para correlationId {CorrelationId}, sagaId {SagaId}", 
            correlationId, sagaId);
        
        // Get the existing correlation info
        var correlationInfo = await _correlationRepository.GetCorrelationAsync(correlationId);
        
        if (correlationInfo == null)
        {
            _logger.LogWarning("No se encontró información para correlationId {CorrelationId} al manejar SagaIniciada", 
                correlationId);
            
            // Create a new correlation info if it doesn't exist
            correlationInfo = new CorrelationInfo
            {
                CorrelationId = correlationId,
                ImagenId = imagenId,
                CreatedAt = DateTime.UtcNow
            };
        }
        
        // Update with the saga ID
        correlationInfo.SagaId = sagaId;
        
        // Save the updated correlation info
        await _correlationRepository.SaveCorrelationAsync(correlationInfo, DEFAULT_TTL_SECONDS);
        
        _logger.LogInformation("Actualizada correlación {CorrelationId} con sagaId {SagaId}", 
            correlationId, sagaId);
    }
}
