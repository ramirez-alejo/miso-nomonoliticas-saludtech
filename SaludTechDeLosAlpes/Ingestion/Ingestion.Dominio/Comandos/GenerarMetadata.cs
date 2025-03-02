using Core.Dominio;

namespace Ingestion.Dominio.Comandos;

public class GenerarMetadata
{
	public Guid SagaId { get; set; }
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public string Resolucion { get; set; }
	public string Contraste { get; set; }
	public bool Es3D { get; set; }
	public string FaseEscaner { get; set; }
	public ContextoProcesal ContextoProcesal { get; set; }
	public Demografia Demografia { get; set; }
	public Historial Historial { get; set; }
	public EntornoClinico EntornoClinico { get; set; }
	public List<Sintoma> Sintomas { get; set; }
	public Dictionary<string,string> Tags { get; set; }
}
