namespace Ingestion.Dominio.Eventos;

/// <summary>
/// Event published when a saga is started
/// </summary>
public class EstadoSaga
{
    /// <summary>
    /// The correlation ID from the original request
    /// </summary>
    public Guid CorrelationId { get; set; }
    
    /// <summary>
    /// The saga ID generated by the Ingestion service
    /// </summary>
    public Guid SagaId { get; set; }
    
    /// <summary>
    /// The image ID being processed
    /// </summary>
    public Guid ImagenId { get; set; }
    
    /// <summary>
    /// When the saga was created
    /// </summary>
    public DateTime FechaCreacion { get; set; }
    public string Status { get; set; }
    
    /// <summary>
    /// Schema version
    /// </summary>
    public string Version { get; set; } = "1.0.0";
}
