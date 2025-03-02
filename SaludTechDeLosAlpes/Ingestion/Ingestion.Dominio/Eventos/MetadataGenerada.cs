namespace Ingestion.Dominio.Eventos;

public class MetadataGenerada
{
	public Guid SagaId { get; set; }
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public Dictionary<string,string> Tags { get; set; }
	public bool Success { get; set; } = true;
	public string ErrorMessage { get; set; }
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
