using Core.Dominio;

namespace Ingestion.Dominio.Eventos;

public class ImagenIngestionCompletada
{
    public Guid SagaId { get; set; }
    public Guid ImagenId { get; set; }
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
