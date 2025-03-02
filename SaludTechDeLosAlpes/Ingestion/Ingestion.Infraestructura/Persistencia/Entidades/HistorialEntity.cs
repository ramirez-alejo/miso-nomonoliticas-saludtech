namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class HistorialEntity
{
	public Guid Id { get; set; }
	public bool Fumador { get; set; }
	public bool Diabetico { get; set; }
	public string CondicionesPrevias { get; set; }
}