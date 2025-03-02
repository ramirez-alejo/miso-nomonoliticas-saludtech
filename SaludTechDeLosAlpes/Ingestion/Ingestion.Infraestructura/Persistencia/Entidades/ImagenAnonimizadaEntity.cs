using System.ComponentModel.DataAnnotations.Schema;

namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class ImagenAnonimizadaEntity
{
	public Guid Id { get; set; }
	public string ImagenProcesadaPath { get; set; }
	[ForeignKey("Imagenes")]
	public Guid ImagenId { get; set; }
	public virtual ImagenEntity Imagen { get; set; }
}