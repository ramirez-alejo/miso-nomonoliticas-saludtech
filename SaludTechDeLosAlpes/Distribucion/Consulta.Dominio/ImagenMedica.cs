using Core.Dominio;

namespace Consulta.Dominio;

public class ImagenMedica
{
	public Guid Id { get; set; }
	public string Version { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public ContextoProcesal ContextoProcesal { get; set; }
	public Metadatos Metadatos { get; set; }	
	public Demografia Demografia { get; set; }
}