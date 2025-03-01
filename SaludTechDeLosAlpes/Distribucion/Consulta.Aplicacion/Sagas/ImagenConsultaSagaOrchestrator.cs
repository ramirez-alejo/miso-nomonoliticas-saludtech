using Consulta.Aplicacion.Consultas;
using Consulta.Aplicacion.Dtos;
using Consulta.Aplicacion.Mapeo;
using Consulta.Aplicacion.Sagas.Events;
using Consulta.Dominio;
using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Consulta.Aplicacion.Sagas;

public class ImagenConsultaSagaOrchestrator
{
    private readonly IMessageProducer _messageProducer;
    private readonly ISagaStateRepository _stateRepository;
    private readonly ILogger<ImagenConsultaSagaOrchestrator> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    // Topic constants
    private const string TOPIC_DEMOGRAFIA_REQUEST = "imagen-consulta-demografia-request";
    private const string TOPIC_MODALIDAD_REQUEST = "imagen-consulta-modalidad-request";
    private const string TOPIC_DATA_REQUEST = "imagen-consulta-data-request";
    private const string TOPIC_DATA_RESPONSE = "imagen-consulta-data-response";
    private const string TOPIC_CONSULTA_COMPLETED = "imagen-consulta-completed";

    public ImagenConsultaSagaOrchestrator(
        IMessageProducer messageProducer,
        ISagaStateRepository stateRepository,
        ILogger<ImagenConsultaSagaOrchestrator> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _messageProducer = messageProducer;
        _stateRepository = stateRepository;
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<Guid> StartSaga(ImagenMedicaConsultaConFiltros request)
    {
        // Create new saga state
        var sagaId = Guid.NewGuid();
        var state = new ImagenConsultaSagaState
        {
            SagaId = sagaId,
            Status = "Started",
            CreatedAt = DateTime.UtcNow,
            DemografiaFilter = request.Demografia,
            TipoImagenFilter = request.TipoImagen
        };

        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Started new saga with ID {SagaId}", sagaId);

        // Publish events to start the process
        await _messageProducer.SendJsonAsync(TOPIC_DEMOGRAFIA_REQUEST, new ImagenConsultaDemografiaRequestEvent
        {
            SagaId = sagaId,
            Filter = request.Demografia
        });

        await _messageProducer.SendJsonAsync(TOPIC_MODALIDAD_REQUEST, new ImagenConsultaModalidadRequestEvent
        {
            SagaId = sagaId,
            Filter = request.TipoImagen
        });

        return sagaId;
    }

    public async Task HandleDemografiaResponse(ImagenConsultaDemografiaResponseEvent response)
    {
        var state = await _stateRepository.GetStateAsync(response.SagaId);
        if (state == null)
        {
            _logger.LogWarning("Received demografia response for unknown saga {SagaId}", response.SagaId);
            return;
        }

        if (!response.Success)
        {
            await HandleFailure(state, response.ErrorMessage ?? "Demografia service failed");
            return;
        }

        // Update state
        state.DemografiaResults = response.ImagenIds;
        state.Status = "DemografiaCompleted";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Demografia step completed for saga {SagaId}", response.SagaId);

        // Check if we can proceed to the next step
        await TryProceedToIntersection(state);
    }

    public async Task HandleModalidadResponse(ImagenConsultaModalidadResponseEvent response)
    {
        var state = await _stateRepository.GetStateAsync(response.SagaId);
        if (state == null)
        {
            _logger.LogWarning("Received modalidad response for unknown saga {SagaId}", response.SagaId);
            return;
        }

        if (!response.Success)
        {
            await HandleFailure(state, response.ErrorMessage ?? "Modalidad service failed");
            return;
        }

        // Update state
        state.ModalidadResults = response.ImagenIds;
        state.Status = "ModalidadCompleted";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Modalidad step completed for saga {SagaId}", response.SagaId);

        // Check if we can proceed to the next step
        await TryProceedToIntersection(state);
    }

    private async Task TryProceedToIntersection(ImagenConsultaSagaState state)
    {
        // Check if both demografia and modalidad steps are completed
        if (state.DemografiaResults != null && state.ModalidadResults != null)
        {
            // Perform intersection
            var intersectedIds = state.DemografiaResults.Intersect(state.ModalidadResults).ToArray();
            state.IntersectedIds = intersectedIds;
            state.Status = "IntersectionCompleted";
            await _stateRepository.SaveStateAsync(state);
            _logger.LogInformation("Intersection completed for saga {SagaId}, found {Count} matching images", 
                state.SagaId, intersectedIds.Length);

            if (intersectedIds.Length == 0)
            {
                // No matching images, complete the saga with empty result
                await CompleteSaga(state, Array.Empty<ImagenMedica>());
                return;
            }

            // Proceed to data fetch step
            await _messageProducer.SendJsonAsync(TOPIC_DATA_REQUEST, new ImagenConsultaDataRequestEvent
            {
                SagaId = state.SagaId,
                ImagenIds = intersectedIds
            });
        }
    }

    public async Task HandleDataResponse(ImagenConsultaDataResponseEvent response)
    {
        var state = await _stateRepository.GetStateAsync(response.SagaId);
        if (state == null)
        {
            _logger.LogWarning("Received data response for unknown saga {SagaId}", response.SagaId);
            return;
        }

        if (!response.Success)
        {
            await HandleFailure(state, response.ErrorMessage ?? "Data warehouse service failed");
            return;
        }

        // Update state
        state.FinalResults = response.Imagenes;
        state.Status = "DataFetched";
        await _stateRepository.SaveStateAsync(state);
        _logger.LogInformation("Data fetch completed for saga {SagaId}, retrieved {Count} images", 
            response.SagaId, response.Imagenes.Length);

        // Complete the saga
        await CompleteSaga(state, response.Imagenes);
    }

    private async Task CompleteSaga(ImagenConsultaSagaState state, ImagenMedica[] imagenes)
    {
        state.FinalResults = imagenes;
        state.Status = "Completed";
        state.CompletedAt = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state);

        // Publish completion event
        await _messageProducer.SendJsonAsync(TOPIC_CONSULTA_COMPLETED, new ImagenConsultaCompletedEvent
        {
            SagaId = state.SagaId,
            Imagenes = imagenes,
            Success = true
        });

        _logger.LogInformation("Saga {SagaId} completed successfully", state.SagaId);
    }

    private async Task HandleFailure(ImagenConsultaSagaState state, string errorMessage)
    {
        state.Status = "Failed";
        state.ErrorMessage = errorMessage;
        state.CompletedAt = DateTime.UtcNow;
        await _stateRepository.SaveStateAsync(state);

        // Publish completion event with failure
        await _messageProducer.SendJsonAsync(TOPIC_CONSULTA_COMPLETED, new ImagenConsultaCompletedEvent
        {
            SagaId = state.SagaId,
            Imagenes = Array.Empty<ImagenMedica>(),
            Success = false,
            ErrorMessage = errorMessage
        });

        _logger.LogError("Saga {SagaId} failed: {ErrorMessage}", state.SagaId, errorMessage);
    }

    // These methods have been removed as they are no longer needed.
    // The Demografia and Modalidad services now handle the requests directly via their own workers.

    // Method to handle data warehouse requests asynchronously
    public async Task HandleDataRequest(Guid sagaId, Guid[] imagenIds)
    {
        try
        {
            // Update saga state
            var state = await _stateRepository.GetStateAsync(sagaId);
            if (state == null)
            {
                _logger.LogWarning("Received data request for unknown saga {SagaId}", sagaId);
                return;
            }

            state.Status = "DataRequested";
            await _stateRepository.SaveStateAsync(state);

            // Create a data warehouse request event
            var dataWarehouseRequest = new ImagenConsultaDataWarehouseRequestEvent
            {
                SagaId = sagaId,
                ImagenIds = imagenIds
            };

            // Publish the event to the data warehouse service
            await _messageProducer.SendJsonAsync("imagen-consulta-datawarehouse-request", dataWarehouseRequest);
            _logger.LogInformation("Published data warehouse request for saga {SagaId}", sagaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing data warehouse request for saga {SagaId}", sagaId);
            
            // Publish failure event
            await _messageProducer.SendJsonAsync(TOPIC_DATA_RESPONSE, new ImagenConsultaDataResponseEvent
            {
                SagaId = sagaId,
                Imagenes = Array.Empty<ImagenMedica>(),
                Success = false,
                ErrorMessage = ex.Message
            });
        }
    }
}
