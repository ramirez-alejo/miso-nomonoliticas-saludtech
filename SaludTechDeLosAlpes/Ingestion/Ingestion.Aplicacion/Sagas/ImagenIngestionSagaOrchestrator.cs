using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Dtos;
using Ingestion.Aplicacion.Mapeo;
using Ingestion.Dominio.Comandos;
using Ingestion.Dominio.Eventos;
using Ingestion.Infraestructura.Persistencia.Repositorios;
using Microsoft.Extensions.Logging;

namespace Ingestion.Aplicacion.Sagas;

public class ImagenIngestionSagaOrchestrator
{
    private readonly IMessageProducer _messageProducer;
    private readonly ISagaStateRepository _stateRepository;
    private readonly IImagenRepository _imagenRepository;
    private readonly ILogger<ImagenIngestionSagaOrchestrator> _logger;

    // Topic constants
    private const string TOPIC_ANONIMIZAR = "imagen-anonimizar";
    private const string TOPIC_METADATA = "metadata-imagen";
    private const string TOPIC_INGESTION_COMPLETED = "imagen-ingestion-completed";

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
    }

    public async Task<Guid> StartSaga(ImagenDto imagenDto)
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
            await HandleFailure(state, evento.ErrorMessage ?? "Anonimización failed");
            return;
        }

        // Update state
        state.AnonimizacionCompleted = true;
        state.ImagenProcesadaPath = evento.ImagenProcesadaPath;
        state.Status = "AnonimizacionCompleted";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Anonimización completed for saga {SagaId}", evento.SagaId);

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
            await HandleFailure(state, evento.ErrorMessage ?? "Metadata generation failed");
            return;
        }

        // Update state
        state.MetadataCompleted = true;
        state.Tags = evento.Tags;
        state.Status = "MetadataCompleted";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Metadata generation completed for saga {SagaId}", evento.SagaId);

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
    }

    private async Task HandleFailure(ImagenIngestionSagaState state, string errorMessage)
    {
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
    }
}
