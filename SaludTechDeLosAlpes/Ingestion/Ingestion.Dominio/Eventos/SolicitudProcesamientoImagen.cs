using Core.Dominio;

namespace Ingestion.Dominio.Eventos;

/// <summary>
/// Event to request image processing from the BFF to the Ingestion service
/// </summary>
public class SolicitudProcesamientoImagen : Imagen
{
    /// <summary>
    /// The correlation ID for tracking the request
    /// </summary>
    public Guid CorrelationId { get; set; }
}
