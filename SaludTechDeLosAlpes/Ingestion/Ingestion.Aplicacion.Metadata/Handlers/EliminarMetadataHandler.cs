using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;
using Ingestion.Dominio.Comandos;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Metadata.Handlers;

public class EliminarMetadataHandler
{
    private readonly ILogger<EliminarMetadataHandler> _logger;
    private readonly IMetadataRepository _repository;
    private readonly IMessageProducer _messageProducer;
    private const string TOPIC_METADATA_ELIMINADA = "imagen-metadata-eliminada";

    public EliminarMetadataHandler(
        ILogger<EliminarMetadataHandler> logger,
        IMetadataRepository repository,
        IMessageProducer messageProducer)
    {
        _logger = logger;
        _repository = repository;
        _messageProducer = messageProducer;
    }

    public async Task HandleEliminarMetadata(EliminarMetadata comando)
    {
        try
        {
            if (comando == null)
            {
                _logger.LogWarning("Error al procesar el comando de eliminar metadata: comando nulo");
                await PublishFailureEvent(Guid.Empty, Guid.Empty, "Comando nulo");
                return;
            }

            _logger.LogInformation("Procesando eliminar metadata para ImagenId: {ImagenId}", comando.ImagenId);

            // Delete the metadata
            await _repository.DeleteMetadataGenerada(comando.ImagenId, CancellationToken.None);

            _logger.LogInformation("Comando EliminarMetadata procesado para ImagenId: {ImagenId}", comando.ImagenId);

            // Publish success event
            await PublishSuccessEvent(comando.SagaId, comando.ImagenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando comando eliminar metadata");

            // Publish failure event
            if (comando != null)
            {
                await PublishFailureEvent(comando.SagaId, comando.ImagenId, ex.Message);
            }
            else
            {
                await PublishFailureEvent(Guid.Empty, Guid.Empty, ex.Message);
            }
        }
    }

    private async Task PublishSuccessEvent(Guid sagaId, Guid imagenId)
    {
        var evento = new MetadataEliminada
        {
            SagaId = sagaId,
            ImagenId = imagenId,
            Version = "1.0",
            Success = true
        };

        await _messageProducer.SendWithSchemaAsync(TOPIC_METADATA_ELIMINADA, evento);
        _logger.LogInformation("Published success metadata eliminada event for saga {SagaId}", sagaId);
    }

    private async Task PublishFailureEvent(Guid sagaId, Guid imagenId, string errorMessage)
    {
        var evento = new MetadataEliminada
        {
            SagaId = sagaId,
            ImagenId = imagenId,
            Version = "1.0",
            Success = false,
            ErrorMessage = errorMessage
        };

        await _messageProducer.SendWithSchemaAsync(TOPIC_METADATA_ELIMINADA, evento);
        _logger.LogInformation("Published failure metadata eliminada event for saga {SagaId}: {ErrorMessage}",
            sagaId, errorMessage);
    }
}
