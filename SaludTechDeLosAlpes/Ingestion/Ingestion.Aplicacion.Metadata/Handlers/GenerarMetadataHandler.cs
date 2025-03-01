using Ingestion.Aplicacion.Metadata.Mapeo;
using Ingestion.Aplicacion.Metadata.Persistencia.Repositorios;

namespace Ingestion.Aplicacion.Metadata.Handlers;

public class GenerarMetadataHandler{
	private readonly ILogger<GenerarMetadataHandler> _logger;
	private readonly IMetadataRepository _repository;

	public GenerarMetadataHandler(
		ILogger<GenerarMetadataHandler> logger,
		IMetadataRepository repository)
	{
		_logger = logger;
		_repository = repository;
	}

	public async Task HandleGenerarMetadata(Ingestion.Dominio.Comandos.GenerarMetadata comando)
	{
		try
		{
			if (comando == null)
			{
				_logger.LogWarning("Error al procesar el comando de generar metadata: comando nulo");
				return;
			}

			_logger.LogInformation("Procesando generar metadata de Imagen para ImagenId: {ImagenId}", comando?.ImagenId);
			_logger.LogInformation("Detalle del comando: {@Evento}", comando);

			var metadata = MapeoMetadataImagen.MapFromCommand(comando);
			metadata.GenerarTags();
			await _repository.UpsertMetadataGenerada(metadata, CancellationToken.None);

			_logger.LogInformation("Comando metadata procesado para ImagenId: {ImagenId}",
				comando.ImagenId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error procesando comando metadata");
			throw;
		}
	}
}
