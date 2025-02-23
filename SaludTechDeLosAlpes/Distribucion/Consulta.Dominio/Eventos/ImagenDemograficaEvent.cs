namespace Consulta.Dominio.Eventos;

public record ImagenDemograficaEvent
{
    public Guid ImagenId { get; init; }
    public string GrupoEdad { get; init; }
    public string Sexo { get; init; }
    public string Etnicidad { get; init; }
    public string TokenAnonimo { get; init; }
    public DateTime FechaCreacion { get; init; }
}
