namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class PacienteEntity
{
	public Guid Id { get; set; }
	public DemografiaEntity Demografia { get; set; }
	public HistorialEntity Historial { get; set; }
	public string TokenAnonimo { get; set; }
}