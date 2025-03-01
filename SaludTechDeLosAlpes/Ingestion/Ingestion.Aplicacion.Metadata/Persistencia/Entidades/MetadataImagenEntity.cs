namespace Ingestion.Aplicacion.Metadata.Persistencia.Entidades;

public class MetadataImagenEntity
{
	public Guid Id { get; set; }
	// Version del comando que genero la metadata
	public string Version { get; set; }
	// Version del servicio que genero la metadata
	public string VersionServicio { get; set; }
	public Guid ImagenId { get; set; }
	public string NombreModalidad { get; set; }
	public string DescripcionModalidad { get; set; }
	public string RegionAnatomica { get; set; }
	public string DescripcionRegionAnatomica { get; set; }
	public string DescripcionPatologia { get; set; }
	public string Resolucion { get; set; }
	public string Contraste { get; set; }
	public bool Es3D { get; set; }
	public string FaseEscaner { get; set; }
	public string EtapaContextoProcesal { get; set; }
	public string GrupoEdad { get; set; }
	public string Sexo { get; set; }
	public string Etnicidad { get; set; }
	public bool Fumador { get; set; }
	public bool Diabetico { get; set; }
	public string CondicionesPrevias { get; set; }
	public string TipoAmbiente { get; set; }
	public string Sintomas { get; set; }
	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}