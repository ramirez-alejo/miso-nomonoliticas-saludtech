using Consulta.Dominio;

namespace Consulta.Aplicacion.Modalidad.Sagas.Events;

// Request event
public class ImagenConsultaModalidadRequestEvent
{
    public Guid SagaId { get; set; }
    public ImagenTipoImagen Filter { get; set; }
}

// Response event
public class ImagenConsultaModalidadResponseEvent
{
    public Guid SagaId { get; set; }
    public Guid[] ImagenIds { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
