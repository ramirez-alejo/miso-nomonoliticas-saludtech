
using Core.Dominio;

namespace Ingestion.Aplicacion.Anonimizacion.Modelos;

public class Anonimizar
{
	public string Version { get; set; }
	public Guid ImagenId { get; set; }
	public TipoImagen TipoImagen { get; set; }
	public AtributosImagen AtributosImagen { get; set; }
	public string UbicacionImagen { get; set; }
	public string ImagenProcesadaPath { get; set; } = string.Empty;
	public string DetalleAnonimizacion { get; set; } = string.Empty;
	
	
	
	
	public void AnonimizarImagen()
	{
		if (AtributosImagen?.Resolucion?.Contains('$') ==  true) throw new Exception("Resolucion de imagen no valida");
		// Anonimizar la imagen
		ImagenProcesadaPath = $"Imagenes/Anonimizadas/{Guid.NewGuid()}.png";
		DetalleAnonimizacion = $"Se realizo la anonimizacion de la imagen {ImagenId} agregando filtro borroso a la parte inferior ...";
		
	}
}