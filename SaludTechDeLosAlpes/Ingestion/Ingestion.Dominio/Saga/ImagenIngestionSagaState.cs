using System.Text.Json.Serialization;

namespace Ingestion.Dominio.Saga;

public class ImagenIngestionSagaState
{
    public Guid SagaId { get; set; }
    public string Status { get; set; } // "Started", "AnonimizacionCompleted", "MetadataCompleted", "Completed", "Failed"
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid ImagenId { get; set; }
    public bool AnonimizacionCompleted { get; set; }
    public bool MetadataCompleted { get; set; }
    public string ImagenProcesadaPath { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public bool HasError { get; set; }
    public string ErrorMessage { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]   
    public Guid? CorrelationId { get; set; }
}
