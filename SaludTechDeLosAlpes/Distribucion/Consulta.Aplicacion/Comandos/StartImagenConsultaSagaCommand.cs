using Consulta.Aplicacion.Consultas;
using Consulta.Aplicacion.Sagas;
using Mediator;

namespace Consulta.Aplicacion.Comandos;

public record StartImagenConsultaSagaCommand(ImagenMedicaConsultaConFiltros Request) : IRequest<Guid>;

public class StartImagenConsultaSagaCommandHandler : IRequestHandler<StartImagenConsultaSagaCommand, Guid>
{
    private readonly ImagenConsultaSagaOrchestrator _orchestrator;
    private readonly ILogger<StartImagenConsultaSagaCommandHandler> _logger;

    public StartImagenConsultaSagaCommandHandler(
        ImagenConsultaSagaOrchestrator orchestrator,
        ILogger<StartImagenConsultaSagaCommandHandler> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    public async ValueTask<Guid> Handle(StartImagenConsultaSagaCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting imagen consulta saga");
        return await _orchestrator.StartSaga(command.Request);
    }
}
