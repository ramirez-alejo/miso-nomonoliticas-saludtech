namespace Consulta.Dominio;

public class ImagenTipoImagen
{
	public Guid ImagenId { get; set; }
	public Modalidad Modalidad { get; set; }
	public RegionAnatomica RegionAnatomica { get; set; }
	
}

public class Modalidad
{
	public string Nombre { get; set; }
	public string Descripcion { get; set; }
}

public class RegionAnatomica
{
	// E.g., "Cabeza y cuello", "Tórax", "Abdomen", etc.
	public string Nombre { get; set; }
	public string Descripcion { get; set; }
}