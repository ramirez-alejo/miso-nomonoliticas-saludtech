namespace Ingestion.Infraestructura.Persistencia.Entidades;

public class TipoImagenEntity
{
	public Guid Id { get; set; }
	public ModalidadEntity Modalidad { get; set; }
	public RegionAnatomicaEntity RegionAnatomica { get; set; }
	public PatologiaEntity Patologia { get; set; }
}