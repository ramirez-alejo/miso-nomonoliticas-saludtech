using Consulta.Aplicacion.Dtos;

namespace Ingestion.Dominio.Comandos;

public class Anonimizar
{
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public string Resolucion { get; set; }
	public string Contraste { get; set; }
	public bool Es3D { get; set; }
	public string FaseEscaner { get; set; }

	public string UbicacionImagen { get; set; }

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}