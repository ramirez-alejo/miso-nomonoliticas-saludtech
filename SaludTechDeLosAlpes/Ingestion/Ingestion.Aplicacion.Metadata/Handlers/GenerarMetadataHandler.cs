using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Metadata.Mapeo;
using Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Metadata.Handlers;

public class GenerarMetadataHandler{
	private readonly ILogger<GenerarMetadataHandler> _logger;
	private readonly IMetadataRepository _repository;
	private readonly IMessageProducer _messageProducer;
	private const string TOPIC_METADATA_GENERADA = "imagen-metadata-generada";

	public GenerarMetadataHandler(
		ILogger<GenerarMetadataHandler> logger,
		IMetadataRepository repository,
		IMessageProducer messageProducer)
	{
		_logger = logger;
		_repository = repository;
		_messageProducer = messageProducer;
	}

	public async Task HandleGenerarMetadata(Ingestion.Dominio.Comandos.GenerarMetadata comando)
	{
		try
		{
			if (comando == null)
			{
				_logger.LogWarning("Error al procesar el comando de generar metadata: comando nulo");
				
				// Publish failure event
				await PublishFailureEvent(Guid.Empty, Guid.Empty, "Comando nulo");
				return;
			}

			_logger.LogInformation("Procesando generar metadata de Imagen para ImagenId: {ImagenId}", comando?.ImagenId);
			_logger.LogInformation("Detalle del comando: {@Evento}", comando);
			

			var metadata = MapeoMetadataImagen.MapFromCommand(comando);
			metadata.GenerarTags();
			await _repository.InsertMetadataGenerada(metadata, CancellationToken.None);

			_logger.LogInformation("Comando metadata procesado para ImagenId: {ImagenId}",
				comando.ImagenId);
				
			// Publish success event
			await PublishSuccessEvent(comando.SagaId, comando.ImagenId, metadata.Tags);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error procesando comando metadata");
			
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
	
	private async Task PublishSuccessEvent(Guid sagaId, Guid imagenId, Dictionary<string, string> tags)
	{
		var evento = new MetadataGenerada
		{
			SagaId = sagaId,
			ImagenId = imagenId,
			Version = "1.0",
			Tags = tags,
			Success = true
		};
		
		await _messageProducer.SendWithSchemaAsync(TOPIC_METADATA_GENERADA, evento);
		_logger.LogInformation("Published success metadata generada event for saga {SagaId}", sagaId);
	}
	
	private async Task PublishFailureEvent(Guid sagaId, Guid imagenId, string errorMessage)
	{
		var evento = new MetadataGenerada
		{
			SagaId = sagaId,
			ImagenId = imagenId,
			Version = "1.0",
			Tags = new Dictionary<string, string>(),
			Success = false,
			ErrorMessage = errorMessage
		};
		
		await _messageProducer.SendWithSchemaAsync(TOPIC_METADATA_GENERADA, evento);
		_logger.LogInformation("Published failure metadata generada event for saga {SagaId}: {ErrorMessage}", 
			sagaId, errorMessage);
	}
}
