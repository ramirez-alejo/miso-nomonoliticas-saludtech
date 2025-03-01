
namespace Ingestion.Dominio.Eventos;

public class Anonimizada
{
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public string ImagenProcesadaPath { get; set; }
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}