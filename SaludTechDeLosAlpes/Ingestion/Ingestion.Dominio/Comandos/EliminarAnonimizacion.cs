namespace Ingestion.Dominio.Comandos;

public class EliminarAnonimizacion
{
    public Guid SagaId { get; set; }
    public string Version { get; set; }
    public Guid ImagenId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
