using Consulta.Dominio;
using Core.Dominio;

namespace Consulta.Aplicacion.Sagas.Events;

// Request events
public class ImagenConsultaDemografiaRequestEvent
{
    public Guid SagaId { get; set; }
    public ImagenDemografia Filter { get; set; }
}

public class ImagenConsultaModalidadRequestEvent
{
    public Guid SagaId { get; set; }
    public ImagenTipoImagen Filter { get; set; }
}

public class ImagenConsultaDataRequestEvent
{
    public Guid SagaId { get; set; }
    public Guid[] ImagenIds { get; set; }
}

// Response events
public class ImagenConsultaDemografiaResponseEvent
{
    public Guid SagaId { get; set; }
    public Guid[] ImagenIds { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ImagenConsultaModalidadResponseEvent
{
    public Guid SagaId { get; set; }
    public Guid[] ImagenIds { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ImagenConsultaDataResponseEvent
{
    public Guid SagaId { get; set; }
    public ImagenMedica[] Imagenes { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

// Completion event
public class ImagenConsultaCompletedEvent
{
    public Guid SagaId { get; set; }
    public ImagenMedica[] Imagenes { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
