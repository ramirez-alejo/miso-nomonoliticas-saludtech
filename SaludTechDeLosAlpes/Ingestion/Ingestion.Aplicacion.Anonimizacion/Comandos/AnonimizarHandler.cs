using Core.Infraestructura.MessageBroker;
using Ingestion.Aplicacion.Anonimizacion.Mapeo;
using Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios;
using Ingestion.Dominio.Eventos;

namespace Ingestion.Aplicacion.Anonimizacion.Comandos
{
    public class AnonimizarHandler
    {
        private readonly ILogger<AnonimizarHandler> _logger;
        private readonly IAnonimizarRepository _repository;
        private readonly IMessageProducer _messageProducer;
        private const string TOPIC_ANONIMIZADA = "imagen-anonimizada";

        public AnonimizarHandler(
            ILogger<AnonimizarHandler> logger,
            IAnonimizarRepository repository,
            IMessageProducer messageProducer)
        {
            _logger = logger;
            _repository = repository;
            _messageProducer = messageProducer;
        }

        public async Task HandleAnonimizar(Ingestion.Dominio.Comandos.Anonimizar comando)
        {
            try
            {
                if (comando == null)
                {
                    _logger.LogWarning("Error al procesar el comando de anonimizar de imagen: comando nulo");
                    
                    // Publish failure event
                    await PublishFailureEvent(Guid.Empty, Guid.Empty, "Comando nulo");
                    return;
                }

                _logger.LogInformation("Procesando anonimizar de Imagen para ImagenId: {ImagenId}", comando?.ImagenId);
                _logger.LogInformation("Detalle del comando: {@Evento}", comando);

                var anonimizar = MapeoAnonimizarImagen.MapToModel(comando);
                anonimizar.AnonimizarImagen();
                await _repository.SaveAsync(anonimizar, CancellationToken.None);

                _logger.LogInformation("Comando Anonimizar procesado para ImagenId: {ImagenId}",
                    comando.ImagenId);
                    
                // Publish success event
                await PublishSuccessEvent(comando.SagaId, comando.ImagenId, anonimizar.ImagenProcesadaPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando comando anonimizar");
                
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
        
        private async Task PublishSuccessEvent(Guid sagaId, Guid imagenId, string imagenProcesadaPath)
        {
            var evento = new Anonimizada
            {
                SagaId = sagaId,
                ImagenId = imagenId,
                Version = "1.0",
                ImagenProcesadaPath = imagenProcesadaPath,
                Success = true
            };
            
            await _messageProducer.SendJsonAsync(TOPIC_ANONIMIZADA, evento);
            _logger.LogInformation("Published success anonimizada event for saga {SagaId}", sagaId);
        }
        
        private async Task PublishFailureEvent(Guid sagaId, Guid imagenId, string errorMessage)
        {
            var evento = new Anonimizada
            {
                SagaId = sagaId,
                ImagenId = imagenId,
                Version = "1.0",
                Success = false,
                ErrorMessage = errorMessage
            };
            
            await _messageProducer.SendJsonAsync(TOPIC_ANONIMIZADA, evento);
            _logger.LogInformation("Published failure anonimizada event for saga {SagaId}: {ErrorMessage}", 
                sagaId, errorMessage);
        }
    }
}
