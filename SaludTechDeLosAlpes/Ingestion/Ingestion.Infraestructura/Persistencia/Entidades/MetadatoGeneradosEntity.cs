using System.ComponentModel.DataAnnotations.Schema;

namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class MetadatoGeneradosEntity
{
	public Guid Id { get; set; }
	public string Key { get; set; }
	public string Value { get; set; }
	[ForeignKey("Imagenes")]
	public Guid ImagenId { get; set; }
	public virtual ImagenEntity Imagen { get; set; }
}