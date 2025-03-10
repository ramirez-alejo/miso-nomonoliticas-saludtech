using Ingestion.Dominio.Saga;

namespace BFF.API.Persistence;

/// <summary>
/// Repository interface for managing correlation information
/// </summary>
public interface ICorrelationRepository
{
    /// <summary>
    /// Saves correlation information
    /// </summary>
    /// <param name="correlationInfo">The correlation information to save</param>
    /// <param name="ttl">Optional time-to-live in seconds</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task SaveCorrelationAsync(CorrelationInfo correlationInfo, int? ttl = null);
    
    /// <summary>
    /// Gets correlation information by correlation ID
    /// </summary>
    /// <param name="correlationId">The correlation ID to look up</param>
    /// <returns>The correlation information or null if not found</returns>
    Task<CorrelationInfo> GetCorrelationAsync(Guid correlationId);
    
    /// <summary>
    /// Deletes correlation information
    /// </summary>
    /// <param name="correlationId">The correlation ID to delete</param>
    /// <returns>Task representing the asynchronous operation</returns>
    Task DeleteCorrelationAsync(Guid correlationId);
}
