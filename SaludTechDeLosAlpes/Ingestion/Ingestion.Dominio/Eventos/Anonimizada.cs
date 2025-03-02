
namespace Ingestion.Dominio.Eventos;

public class Anonimizada
{
	public Guid SagaId { get; set; }
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public string ImagenProcesadaPath { get; set; }
	public bool Success { get; set; } = true;
	public string ErrorMessage { get; set; }
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
