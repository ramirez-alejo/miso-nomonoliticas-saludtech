using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios;
using Ingestion.Dominio.Comandos;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Anonimizacion.Comandos
{
    public class EliminarAnonimizacionHandler
    {
        private readonly ILogger<EliminarAnonimizacionHandler> _logger;
        private readonly IAnonimizarRepository _repository;
        private readonly IMessageProducer _messageProducer;
        private const string TOPIC_ANONIMIZACION_ELIMINADA = "imagen-anonimizacion-eliminada";

        public EliminarAnonimizacionHandler(
            ILogger<EliminarAnonimizacionHandler> logger,
            IAnonimizarRepository repository,
            IMessageProducer messageProducer)
        {
            _logger = logger;
            _repository = repository;
            _messageProducer = messageProducer;
        }

        public async Task HandleEliminarAnonimizacion(EliminarAnonimizacion comando)
        {
            try
            {
                if (comando == null)
                {
                    _logger.LogWarning("Error al procesar el comando de eliminar anonimización: comando nulo");
                    await PublishFailureEvent(Guid.Empty, Guid.Empty, "Comando nulo");
                    return;
                }

                _logger.LogInformation("Procesando eliminar anonimización para ImagenId: {ImagenId}", comando.ImagenId);

                // Delete the anonimization data
                await _repository.DeleteAsync(comando.ImagenId, CancellationToken.None);

                _logger.LogInformation("Comando EliminarAnonimizacion procesado para ImagenId: {ImagenId}", comando.ImagenId);

                // Publish success event
                await PublishSuccessEvent(comando.SagaId, comando.ImagenId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando comando eliminar anonimización");

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
            var evento = new AnonimizacionEliminada
            {
                SagaId = sagaId,
                ImagenId = imagenId,
                Version = "1.0",
                Success = true
            };

            await _messageProducer.SendWithSchemaAsync(TOPIC_ANONIMIZACION_ELIMINADA, evento);
            _logger.LogInformation("Published success anonimizacion eliminada event for saga {SagaId}", sagaId);
        }

        private async Task PublishFailureEvent(Guid sagaId, Guid imagenId, string errorMessage)
        {
            var evento = new AnonimizacionEliminada
            {
                SagaId = sagaId,
                ImagenId = imagenId,
                Version = "1.0",
                Success = false,
                ErrorMessage = errorMessage
            };

            await _messageProducer.SendWithSchemaAsync(TOPIC_ANONIMIZACION_ELIMINADA, evento);
            _logger.LogInformation("Published failure anonimizacion eliminada event for saga {SagaId}: {ErrorMessage}",
                sagaId, errorMessage);
        }
    }
}
