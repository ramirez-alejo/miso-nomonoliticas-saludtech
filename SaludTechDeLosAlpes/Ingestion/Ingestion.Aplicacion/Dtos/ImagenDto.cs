namespace Ingestion.Aplicacion.Dtos;

public class ImagenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string Url { get; set; }
}
