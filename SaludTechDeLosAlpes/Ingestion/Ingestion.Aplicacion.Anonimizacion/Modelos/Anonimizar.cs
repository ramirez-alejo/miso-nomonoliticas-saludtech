using Consulta.Aplicacion.Dtos;

namespace Ingestion.Aplicacion.Anonimizacion.Modelos;

public class Anonimizar
{
	public Guid ImagenId { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public string UbicacionImagen { get; set; }
	public string ImagenProcesadaPath { get; set; }
	public string DetalleAnonimizacion { get; set; }
	
	
	
	
	public void AnonimizarImagen()
	{
		// Anonimizar la imagen
		ImagenProcesadaPath = $"Imagenes/Anonimizadas/{Guid.NewGuid()}.png";
		DetalleAnonimizacion = $"Se realizo la anonimizacion de la imagen {ImagenId} agregando filtro borroso a la parte inferior ...";
		
	}
}