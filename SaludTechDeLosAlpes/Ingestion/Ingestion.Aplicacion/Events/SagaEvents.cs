using Core.Dominio;

namespace Ingestion.Aplicacion.Events;

// Request events
public class ImagenConsultaDataWarehouseRequestEvent
{
    public Guid SagaId { get; set; }
    public Guid[] ImagenIds { get; set; }
}

// Response events
public class ImagenConsultaDataResponseEvent
{
    public Guid SagaId { get; set; }
    public Imagen[] Imagenes { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
