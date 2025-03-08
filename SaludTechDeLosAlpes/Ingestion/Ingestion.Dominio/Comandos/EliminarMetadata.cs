namespace Ingestion.Dominio.Comandos;

public class EliminarMetadata
{
    public Guid SagaId { get; set; }
    public string Version { get; set; }
    public Guid ImagenId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
