using Consulta.Dominio;
using Core.Dominio;

namespace Consulta.Aplicacion.Sagas;

public class ImagenConsultaSagaState
{
    public Guid SagaId { get; set; }
    public string Status { get; set; } // "Started", "DemografiaCompleted", "ModalidadCompleted", "IntersectionCompleted", "DataFetched", "Completed", "Failed"
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ImagenDemografia DemografiaFilter { get; set; }
    public ImagenTipoImagen TipoImagenFilter { get; set; }
    public Guid[]? DemografiaResults { get; set; }
    public Guid[]? ModalidadResults { get; set; }
    public Guid[]? IntersectedIds { get; set; }
    public ImagenMedica[]? FinalResults { get; set; }
    public string? ErrorMessage { get; set; }
}
