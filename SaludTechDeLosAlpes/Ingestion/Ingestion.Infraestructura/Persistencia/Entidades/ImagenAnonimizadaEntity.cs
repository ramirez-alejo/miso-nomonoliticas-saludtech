namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class ImagenAnonimizadaEntity
{
	public Guid Id { get; set; }
	public string ImagenProcesadaPath { get; set; }
	public ImagenEntity Imagen { get; set; }
}