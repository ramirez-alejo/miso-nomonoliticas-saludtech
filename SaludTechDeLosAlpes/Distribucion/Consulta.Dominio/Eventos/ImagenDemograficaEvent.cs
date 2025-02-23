namespace Consulta.Dominio.Eventos;

public record ImagenDemografiaEvent
{
    public Guid ImagenId { get; init; }
    public string GrupoEdad { get; init; }
    public string Sexo { get; init; }
    public string Etnicidad { get; init; }
    public DateTime FechaCreacion { get; init; }
    public string Version { get; set; } = "1.0.0";
}
