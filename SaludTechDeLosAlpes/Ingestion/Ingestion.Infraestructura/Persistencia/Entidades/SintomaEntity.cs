namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class SintomaEntity
{
	public Guid Id { get; set; }
	public string Descripcion { get; set; }
	public ICollection<MetadatosEntity> Metadatos { get; set; } = new List<MetadatosEntity>();
}