namespace Consulta.Aplicacion.Modalidad.Persistencia.Entidades;

public class ImagenModalidadEntity
{
    public Guid Id { get; set; }
    public Guid ImagenId { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string RegionAnatomica { get; set; }
    public string RegionDescripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
}
