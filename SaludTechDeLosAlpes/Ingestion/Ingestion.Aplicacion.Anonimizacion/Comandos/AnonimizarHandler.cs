using Ingestion.Aplicacion.Anonimizacion.Mapeo;
using Ingestion.Aplicacion.Anonimizacion.Persistencia.Repositorios;

namespace Ingestion.Aplicacion.Anonimizacion.Comandos
{
    public class AnonimizarHandler
    {
        private readonly ILogger<AnonimizarHandler> _logger;
        private readonly IAnonimizarRepository _repository;

        public AnonimizarHandler(
            ILogger<AnonimizarHandler> logger,
            IAnonimizarRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task HandleAnonimizar(Ingestion.Dominio.Comandos.Anonimizar comando)
        {
            try
            {

                if (comando == null)
                {
                    _logger.LogWarning("Error al procesar el comando de anonimizar de imagen: comando nulo");
                    return;
                }

                _logger.LogInformation("Procesando anonimizar de Imagen para ImagenId: {ImagenId}", comando?.ImagenId);
                _logger.LogInformation("Detalle del comando: {@Evento}", comando);

                var anonimizar = MapeoAnonimizarImagen.MapToModel(comando);
                anonimizar.AnonimizarImagen();
                await _repository.SaveAsync(anonimizar, CancellationToken.None);

                _logger.LogInformation("Comando Anonimizar procesado para ImagenId: {ImagenId}",
                    comando.ImagenId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando comando anonimizar");
                throw;
            }
        }
    }
}
