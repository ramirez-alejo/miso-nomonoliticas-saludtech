namespace Consulta.Dominio.Eventos;

public record ImagenModalidadEvent
{
    public Guid ImagenId { get; init; }
    public string Nombre { get; init; }
    public string Descripcion { get; init; }
    public string RegionAnatomica { get; init; }
    public string RegionDescripcion { get; init; }
    public DateTime FechaCreacion { get; init; }
    public string Version { get; set; } = "1.0.0";
}
