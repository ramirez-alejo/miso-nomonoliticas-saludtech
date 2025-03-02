namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class AtributosImagenEntity
{
	public Guid Id { get; set; }
	public string Resolucion { get; set; }
	public string Contraste { get; set; }
	public bool Es3D { get; set; }
	public string FaseEscaner { get; set; }
}