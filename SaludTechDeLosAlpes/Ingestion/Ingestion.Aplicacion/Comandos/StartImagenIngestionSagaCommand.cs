using Ingestion.Aplicacion.Dtos;
using Mediator;

namespace Ingestion.Aplicacion.Comandos;

public record StartImagenIngestionSagaCommand(ImagenDto Imagen, Guid? CorrelationId = null) : ICommand<Guid>;

public class StartImagenIngestionSagaCommandHandler : ICommandHandler<StartImagenIngestionSagaCommand, Guid>
{
    private readonly Sagas.ImagenIngestionSagaOrchestrator _orchestrator;
    private readonly ILogger<StartImagenIngestionSagaCommandHandler> _logger;

    public StartImagenIngestionSagaCommandHandler(
        Sagas.ImagenIngestionSagaOrchestrator orchestrator,
        ILogger<StartImagenIngestionSagaCommandHandler> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public async ValueTask<Guid> Handle(StartImagenIngestionSagaCommand command, CancellationToken cancellationToken)
    {
        if (command.CorrelationId.HasValue)
        {
            _logger.LogInformation("Starting saga for imagen {ImagenId} with correlation ID {CorrelationId}", 
                command.Imagen.Id, command.CorrelationId.Value);
            return await _orchestrator.StartSaga(command.Imagen, command.CorrelationId);
        }
        else
        {
            _logger.LogInformation("Starting saga for imagen {ImagenId} without correlation ID", command.Imagen.Id);
            return await _orchestrator.StartSaga(command.Imagen);
        }
    }
}
