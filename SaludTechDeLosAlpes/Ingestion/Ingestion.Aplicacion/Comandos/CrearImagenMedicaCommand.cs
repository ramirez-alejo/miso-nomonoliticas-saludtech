using Core.Dominio;
using Core.Infraestructura.MessageBroker;
using Ingestion.Infraestructura.Persistencia.Repositorios;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Ingestion.Aplicacion.Comandos;

public record ImagenCreadaMessage(Guid Id, string Tipo, DateTime FechaCreacion);

public class CrearImagenMedicaCommand : Imagen, IRequest<ImagenMedicaResponse>
{
	
}

public class ImagenMedicaResponse
{
	public Guid Id { get; set; }
}

public class CrearImagenMedicaHandler : IRequestHandler<CrearImagenMedicaCommand, ImagenMedicaResponse>
{
    private readonly IImagenRepository _imagenRepository;
    private readonly ILogger<CrearImagenMedicaHandler> _logger;
    private readonly IMessageProducer _messageProducer;
    private const string TOPIC_IMAGEN_CREADA = "imagen-medica";

    public CrearImagenMedicaHandler(
        IImagenRepository imagenRepository,
        ILogger<CrearImagenMedicaHandler> logger,
        IMessageProducer messageProducer)
    {
        _imagenRepository = imagenRepository;
        _logger = logger;
        _messageProducer = messageProducer;
    }

    public async ValueTask<ImagenMedicaResponse> Handle(CrearImagenMedicaCommand command, CancellationToken cancellationToken)
    {
        var result = await _imagenRepository.AddAsync(command, cancellationToken);
        _logger.LogInformation("Imagen medica creada con id {ResultId}", result.Id);

        // Enviar mensaje de imagen creada
        var mensaje = new ImagenCreadaMessage(
            result.Id,
            nameof(ImagenCreadaMessage),
            DateTime.UtcNow
        );

        await _messageProducer.SendJsonAsync(TOPIC_IMAGEN_CREADA, mensaje);
        _logger.LogInformation("Mensaje de imagen creada enviado para id {ResultId} en topic {Topic}", result.Id, TOPIC_IMAGEN_CREADA);

        return new ImagenMedicaResponse { Id = result.Id };
    }
}
