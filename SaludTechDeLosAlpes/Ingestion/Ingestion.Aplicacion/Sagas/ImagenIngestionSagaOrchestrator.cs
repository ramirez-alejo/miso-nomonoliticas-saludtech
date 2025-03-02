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
        var imagenId = Guid.NewGuid();
        
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
            Version = "1.0",
            UbicacionImagen = imagenDto.Url,
            // Add other properties as needed
        };

        await _messageProducer.SendJsonAsync(TOPIC_ANONIMIZAR, anonimizarCommand);
        _logger.LogInformation("Published anonimizar command for saga {SagaId}", sagaId);

        // Create and publish GenerarMetadata command
        var generarMetadataCommand = new GenerarMetadata
        {
            SagaId = sagaId,
            ImagenId = imagenId,
            Version = "1.0",
            // Add other properties as needed
        };

        await _messageProducer.SendJsonAsync(TOPIC_METADATA, generarMetadataCommand);
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
        // Check if both anonimización and metadata generation are completed
        if (state.AnonimizacionCompleted && state.MetadataCompleted)
        {
            try
            {
                // Get the imagen from the repository or create a new one if it doesn't exist
                Imagen imagen;
                try
                {
                    imagen = await _imagenRepository.GetByIdAsync(state.ImagenId, CancellationToken.None);
                }
                catch
                {
                    // Create a new imagen if it doesn't exist
                    imagen = new Imagen
                    {
                        Id = state.ImagenId,
                        Version = "1.0"
                    };
                }

                // Update the imagen with the data from the saga
                // This is a simplified example, you would need to map all the properties
                // from the saga state to the imagen object

                // Save the imagen
                if (imagen.Id == Guid.Empty)
                {
                    await _imagenRepository.AddAsync(imagen, CancellationToken.None);
                }
                else
                {
                    await _imagenRepository.UpdateAsync(imagen, CancellationToken.None);
                }

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
        await _messageProducer.SendJsonAsync(TOPIC_INGESTION_COMPLETED, new ImagenIngestionCompletada
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
        await _messageProducer.SendJsonAsync(TOPIC_INGESTION_COMPLETED, new ImagenIngestionCompletada
        {
            SagaId = state.SagaId,
            ImagenId = state.ImagenId,
            Success = false,
            ErrorMessage = errorMessage
        });

        _logger.LogError("Saga {SagaId} failed: {ErrorMessage}", state.SagaId, errorMessage);
    }
}
