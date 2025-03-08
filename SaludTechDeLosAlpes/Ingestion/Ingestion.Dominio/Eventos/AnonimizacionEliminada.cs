namespace Ingestion.Dominio.Eventos;

public class AnonimizacionEliminada
{
    public Guid SagaId { get; set; }
    public Guid ImagenId { get; set; }
    public string Version { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
