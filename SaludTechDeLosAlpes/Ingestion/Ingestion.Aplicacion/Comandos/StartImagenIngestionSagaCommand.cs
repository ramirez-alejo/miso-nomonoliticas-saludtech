using Ingestion.Aplicacion.Dtos;
using Mediator;

namespace Ingestion.Aplicacion.Comandos;

public record StartImagenIngestionSagaCommand(ImagenDto Imagen) : ICommand<Guid>;

public class StartImagenIngestionSagaCommandHandler : ICommandHandler<StartImagenIngestionSagaCommand, Guid>
{
    private readonly Sagas.ImagenIngestionSagaOrchestrator _orchestrator;

    public StartImagenIngestionSagaCommandHandler(Sagas.ImagenIngestionSagaOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async ValueTask<Guid> Handle(StartImagenIngestionSagaCommand command, CancellationToken cancellationToken)
    {
        return await _orchestrator.StartSaga(command.Imagen);
    }
}
