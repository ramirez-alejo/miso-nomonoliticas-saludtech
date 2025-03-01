namespace Ingestion.Dominio.Eventos;

public class MetadataGenerada
{
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public Dictionary<string,string> Tags { get; set; }
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}