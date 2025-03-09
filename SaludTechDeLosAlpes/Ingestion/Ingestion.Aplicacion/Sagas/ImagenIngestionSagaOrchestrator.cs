using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Dtos;
using Ingestion.Aplicacion.Mapeo;
using Ingestion.Dominio.Comandos;
using Ingestion.Dominio.Eventos;
using Ingestion.Dominio.Saga;
using Ingestion.Infraestructura.Logging;
using Ingestion.Infraestructura.Persistencia.Repositorios;
using Microsoft.Extensions.Logging;

namespace Ingestion.Aplicacion.Sagas;

public class ImagenIngestionSagaOrchestrator
{
    private readonly IMessageProducer _messageProducer;
    private readonly ISagaStateRepository _stateRepository;
    private readonly IImagenRepository _imagenRepository;
    private readonly ILogger<ImagenIngestionSagaOrchestrator> _logger;
    private readonly SagaLogger _sagaLogger;

    // Topic constants
    private const string TOPIC_ANONIMIZAR = "imagen-anonimizar";
    private const string TOPIC_METADATA = "metadata-imagen";
    private const string TOPIC_ELIMINAR_ANONIMIZACION = "imagen-eliminar-anonimizacion";
    private const string TOPIC_ELIMINAR_METADATA = "metadata-eliminar";
    private const string TOPIC_INGESTION_COMPLETED = "imagen-ingestion-completed";
    private const string TOPIC_SAGA_INICIADA = "saga-iniciada";

    public ImagenIngestionSagaOrchestrator(
        IMessageProducer messageProducer,
        ISagaStateRepository stateRepository,
        IImagenRepository imagenRepository,
        ILogger<ImagenIngestionSagaOrchestrator> logger)
    {
        _messageProducer = messageProducer;
        _stateRepository = stateRepository;
        _imagenRepository = imagenRepository;
        _logger = logger;
        _sagaLogger = new SagaLogger(logger);
    }

    /// <summary>
    /// Starts a new ingestion saga
    /// </summary>
    /// <param name="imagenDto">The image data</param>
    /// <returns>The saga ID</returns>
    public async Task<Guid> StartSaga(ImagenDto imagenDto)
    {
        return await StartSaga(imagenDto, null);
    }

    /// <summary>
    /// Starts a new ingestion saga with correlation ID
    /// </summary>
    /// <param name="imagenDto">The image data</param>
    /// <param name="correlationId">Optional correlation ID for tracking</param>
    /// <returns>The saga ID</returns>
    public async Task<Guid> StartSaga(ImagenDto imagenDto, Guid? correlationId)
    {
        // Create new saga state
        var sagaId = Guid.NewGuid();
        var imagenId = imagenDto.Id;
        
        var state = new ImagenIngestionSagaState
        {
            SagaId = sagaId,
            Status = "Started",
            CreatedAt = DateTime.UtcNow,
            ImagenId = imagenId,
            AnonimizacionCompleted = false,
            MetadataCompleted = false
        };

        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Started new ingestion saga with ID {SagaId} for imagen {ImagenId}", sagaId, imagenId);
        _sagaLogger.LogSagaStarted(sagaId, "ImagenIngestion", new Dictionary<string, object>
        {
            ["imagenId"] = imagenId
        });

        // Publish SagaIniciada event if we have a correlation ID
        if (correlationId.HasValue)
        {
            var sagaIniciada = new SagaIniciada
            {
                CorrelationId = correlationId.Value,
                SagaId = sagaId,
                ImagenId = imagenId,
                FechaCreacion = DateTime.UtcNow,
                Version = "1.0.0"
            };

            await _messageProducer.SendWithSchemaAsync(TOPIC_SAGA_INICIADA, sagaIniciada);
            _logger.LogInformation("Published saga iniciada event for saga {SagaId} with correlation ID {CorrelationId}", 
                sagaId, correlationId.Value);
        }

        // Create and publish Anonimizar command
        var anonimizarCommand = new Anonimizar
        {
            SagaId = sagaId,
            ImagenId = imagenId,
            Version = "1.0.0",
            UbicacionImagen = imagenDto.Url,
        };

        await _messageProducer.SendWithSchemaAsync(TOPIC_ANONIMIZAR, anonimizarCommand);
        _logger.LogInformation("Published anonimizar command for saga {SagaId}", sagaId);

        // Create and publish GenerarMetadata command
        var generarMetadataCommand = new GenerarMetadata
        {
            SagaId = sagaId,
            ImagenId = imagenId,
            Version = "1.0.0",
        };

        await _messageProducer.SendWithSchemaAsync(TOPIC_METADATA, generarMetadataCommand);
        _logger.LogInformation("Published generar metadata command for saga {SagaId}", sagaId);

        return sagaId;
    }

    public async Task HandleAnonimizadaEvent(Anonimizada evento)
    {
        var state = await _stateRepository.GetStateAsync(evento.SagaId);
        if (state == null)
        {
            _logger.LogWarning("Received anonimizada event for unknown saga {SagaId}", evento.SagaId);
            return;
        }

        if (!evento.Success)
        {
            _sagaLogger.LogSagaStepFailed(evento.SagaId, "ImagenIngestion", "Anonimizacion", 
                evento.ErrorMessage ?? "Anonimización failed");
            await HandleFailure(state, evento.ErrorMessage ?? "Anonimización failed");
            return;
        }

        // Update state
        state.AnonimizacionCompleted = true;
        state.ImagenProcesadaPath = evento.ImagenProcesadaPath;
        state.Status = "AnonimizacionCompleted";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Anonimización completed for saga {SagaId}", evento.SagaId);
        _sagaLogger.LogSagaStepCompleted(evento.SagaId, "ImagenIngestion", "Anonimizacion", 
            new Dictionary<string, object> { ["imagenProcesadaPath"] = evento.ImagenProcesadaPath });

        // Check if we can complete the saga
        await TryCompleteSaga(state);
    }

    public async Task HandleMetadataGeneradaEvent(MetadataGenerada evento)
    {
        var state = await _stateRepository.GetStateAsync(evento.SagaId);
        if (state == null)
        {
            _logger.LogWarning("Received metadata generada event for unknown saga {SagaId}", evento.SagaId);
            return;
        }

        if (!evento.Success)
        {
            _sagaLogger.LogSagaStepFailed(evento.SagaId, "ImagenIngestion", "Metadata", 
                evento.ErrorMessage ?? "Metadata generation failed");
            await HandleFailure(state, evento.ErrorMessage ?? "Metadata generation failed");
            return;
        }

        // Update state
        state.MetadataCompleted = true;
        state.Tags = evento.Tags;
        state.Status = "MetadataCompleted";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Metadata generation completed for saga {SagaId}", evento.SagaId);
        _sagaLogger.LogSagaStepCompleted(evento.SagaId, "ImagenIngestion", "Metadata", 
            new Dictionary<string, object> { ["tagsCount"] = evento.Tags?.Count ?? 0 });

        // Check if we can complete the saga
        await TryCompleteSaga(state);
    }

    private async Task TryCompleteSaga(ImagenIngestionSagaState state)
    {
        if (state.AnonimizacionCompleted && state.MetadataCompleted)
        {
            try
            {
                
                await _imagenRepository.UpsertImagenAnonimizada(state.ImagenId, state.ImagenProcesadaPath,
                    CancellationToken.None);
                await _imagenRepository.UpsertMetadataGenerada(state.ImagenId, state.Tags, CancellationToken.None);

                // Complete the saga
                await CompleteSaga(state);
            }
            catch (Exception ex)
            {
                await HandleFailure(state, $"Failed to save imagen: {ex.Message}");
            }
        }
    }

    private async Task CompleteSaga(ImagenIngestionSagaState state)
    {
        state.Status = "Completed";
        state.CompletedAt = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state);

        // Publish completion event
        await _messageProducer.SendWithSchemaAsync(TOPIC_INGESTION_COMPLETED, new ImagenIngestionCompletada
        {
            SagaId = state.SagaId,
            ImagenId = state.ImagenId,
            Success = true
        });

        _logger.LogInformation("Saga {SagaId} completed successfully", state.SagaId);
        _sagaLogger.LogSagaCompleted(state.SagaId, "ImagenIngestion", new Dictionary<string, object>
        {
            ["imagenId"] = state.ImagenId,
            ["duration"] = state.CompletedAt.HasValue ? (state.CompletedAt.Value - state.CreatedAt).TotalMilliseconds : 0
        });
    }

    private async Task HandleFailure(ImagenIngestionSagaState state, string errorMessage)
    {
        // Execute compensation actions if needed
        await ExecuteCompensationActions(state);

        // Update state
        state.Status = "Failed";
        state.ErrorMessage = errorMessage;
        state.CompletedAt = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state);

        // Publish completion event with failure
        await _messageProducer.SendWithSchemaAsync(TOPIC_INGESTION_COMPLETED, new ImagenIngestionCompletada
        {
            SagaId = state.SagaId,
            ImagenId = state.ImagenId,
            Success = false,
            ErrorMessage = errorMessage
        });

        _logger.LogError("Saga {SagaId} failed: {ErrorMessage}", state.SagaId, errorMessage);
        _sagaLogger.LogSagaFailed(state.SagaId, "ImagenIngestion", errorMessage, new Dictionary<string, object>
        {
            ["imagenId"] = state.ImagenId,
            ["duration"] = state.CompletedAt.HasValue ? (state.CompletedAt.Value - state.CreatedAt).TotalMilliseconds : 0
        });
    }

    private async Task ExecuteCompensationActions(ImagenIngestionSagaState state)
    {
        // Check which operations have completed and need compensation
        if (state.AnonimizacionCompleted && !state.MetadataCompleted)
        {
            // If Anonimizar succeeded but GenerarMetadata failed, rollback Anonimizar data
            await RollbackAnonimizacionData(state);
        }
        else if (!state.AnonimizacionCompleted && state.MetadataCompleted)
        {
            // If GenerarMetadata succeeded but Anonimizar failed, rollback Metadata data
            await RollbackMetadataData(state);
        }
    }

    private async Task RollbackAnonimizacionData(ImagenIngestionSagaState state)
    {
        try
        {
            _logger.LogInformation("Executing compensation action: Rolling back Anonimizacion data for saga {SagaId}", state.SagaId);
            _sagaLogger.LogCompensationStarted(state.SagaId, "ImagenIngestion", "EliminarAnonimizacion");
            
            // Create and publish EliminarAnonimizacion command
            var eliminarAnonimizacionCommand = new EliminarAnonimizacion
            {
                SagaId = state.SagaId,
                ImagenId = state.ImagenId,
                Version = "1.0.0"
            };

            await _messageProducer.SendWithSchemaAsync(TOPIC_ELIMINAR_ANONIMIZACION, eliminarAnonimizacionCommand);
            
            _logger.LogInformation("Published eliminar anonimizacion command for saga {SagaId}", state.SagaId);
            _sagaLogger.LogCompensationCompleted(state.SagaId, "ImagenIngestion", "EliminarAnonimizacion");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish eliminar anonimizacion command for saga {SagaId}: {ErrorMessage}", 
                state.SagaId, ex.Message);
        }
    }

    private async Task RollbackMetadataData(ImagenIngestionSagaState state)
    {
        try
        {
            _logger.LogInformation("Executing compensation action: Rolling back Metadata data for saga {SagaId}", state.SagaId);
            _sagaLogger.LogCompensationStarted(state.SagaId, "ImagenIngestion", "EliminarMetadata");
            
            // Create and publish EliminarMetadata command
            var eliminarMetadataCommand = new EliminarMetadata
            {
                SagaId = state.SagaId,
                ImagenId = state.ImagenId,
                Version = "1.0.0"
            };

            await _messageProducer.SendWithSchemaAsync(TOPIC_ELIMINAR_METADATA, eliminarMetadataCommand);
            
            _logger.LogInformation("Published eliminar metadata command for saga {SagaId}", state.SagaId);
            _sagaLogger.LogCompensationCompleted(state.SagaId, "ImagenIngestion", "EliminarMetadata");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish eliminar metadata command for saga {SagaId}: {ErrorMessage}", 
                state.SagaId, ex.Message);
        }
    }
}
