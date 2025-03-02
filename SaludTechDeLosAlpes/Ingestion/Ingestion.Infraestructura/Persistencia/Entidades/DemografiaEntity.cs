namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class DemografiaEntity
{
	public Guid Id { get; set; }
	public string GrupoEdad { get; set; }
	public string Sexo { get; set; }
	public string Etnicidad { get; set; }
}