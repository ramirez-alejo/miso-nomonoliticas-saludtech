using BFF.API.Services;
using Core.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;

[ApiController]
[Route("api/bff/imagenes")]
public class ImagenController : ControllerBase
{
    private readonly IImagenService _imagenService;
    private readonly ILogger<ImagenController> _logger;

    public ImagenController(IImagenService imagenService, ILogger<ImagenController> logger)
    {
        _imagenService = imagenService;
        _logger = logger;
    }

    /// <summary>
    /// Inicia el procesamiento de una imagen
    /// </summary>
    /// <param name="request">Datos de la solicitud</param>
    /// <returns>El ID de correlación para seguimiento</returns>
    [HttpPost("procesar")]
    public async Task<IActionResult> IniciarProcesamiento([FromBody] SolicitudProcesamientoRequest request)
    {
        try
        {
            _logger.LogInformation("Recibida solicitud para procesar imagen");

            var correlationId = await _imagenService.IniciarProcesamientoAsync(
                request);
            
            return Accepted(new { CorrelationId = correlationId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar procesamiento para imagen");
            return StatusCode(500, new { Error = "Error al iniciar el procesamiento", Message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el estado de procesamiento de una imagen por ID de correlación
    /// </summary>
    /// <param name="correlationId">ID de correlación</param>
    /// <returns>Estado del procesamiento</returns>
    [HttpGet("estado/{correlationId}")]
    public async Task<IActionResult> ObtenerEstado(Guid correlationId)
    {
        try
        {
            _logger.LogInformation("Recibida solicitud para obtener estado de correlación {CorrelationId}", correlationId);
            
            var correlationInfo = await _imagenService.ObtenerEstadoProcesamientoAsync(correlationId);
            
            if (correlationInfo == null)
            {
                return NotFound(new { Error = "No se encontró información para el ID de correlación proporcionado" });
            }
            
            // Check if we have a saga ID
            if (correlationInfo.SagaId == Guid.Empty)
            {
                return Ok(new 
                { 
                    CorrelationId = correlationInfo.CorrelationId,
                    ImagenId = correlationInfo.ImagenId,
                    Estado = "Pendiente",
                    Mensaje = "La solicitud ha sido recibida y está pendiente de procesamiento"
                });
            }
            
            // query the saga using the id
            var saga = await _imagenService.ObtenerEstadoSagaAsync(correlationInfo.SagaId);
            
            var mensaje = "El procesamiento está en curso";
            
            
            // We have a saga ID, return more detailed information
            return Ok(new 
            { 
                CorrelationId = correlationInfo.CorrelationId,
                SagaId = correlationInfo.SagaId,
                ImagenId = correlationInfo.ImagenId,
                Estado = saga.Status =="Completed" ? "Completado" : "En progreso",
                Mensaje = mensaje,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estado para correlación {CorrelationId}", correlationId);
            return StatusCode(500, new { Error = "Error al obtener el estado", Message = ex.Message });
        }
    }
}

/// <summary>
/// Modelo de solicitud para iniciar el procesamiento de una imagen
/// </summary>
public class SolicitudProcesamientoRequest : Imagen
{
    // Inherits all properties from Core.Dominio.Imagen
}
