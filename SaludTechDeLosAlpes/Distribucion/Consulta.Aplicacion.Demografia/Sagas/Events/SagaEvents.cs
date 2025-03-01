using Consulta.Dominio;

namespace Consulta.Aplicacion.Demografia.Sagas.Events;

// Request event
public class ImagenConsultaDemografiaRequestEvent
{
    public Guid SagaId { get; set; }
    public ImagenDemografia Filter { get; set; }
}

// Response event
public class ImagenConsultaDemografiaResponseEvent
{
    public Guid SagaId { get; set; }
    public Guid[] ImagenIds { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
