namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class MetadatosEntity
{
	public Guid Id { get; set; }
	public EntornoClinicoEntity EntornoClinico { get; set; }
	public ICollection<SintomaEntity> Sintomas { get; set; } = new List<SintomaEntity>();
}