namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class MetadatoGeneradosEntity
{
	public Guid Id { get; set; }
	public string Key { get; set; }
	public string Value { get; set; }
	public ImagenEntity Imagen { get; set; }
}