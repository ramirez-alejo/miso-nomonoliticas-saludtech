using System.Text.Json;
using Ingestion.Dominio.Saga;
using StackExchange.Redis;

namespace BFF.API.Persistence;

/// <summary>
/// Redis implementation of the correlation repository
/// </summary>
public class RedisCorrelationRepository : ICorrelationRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly string _keyPrefix = "bff:correlation:";
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<RedisCorrelationRepository> _logger;

    public RedisCorrelationRepository(string connectionString, ILogger<RedisCorrelationRepository> logger)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task SaveCorrelationAsync(CorrelationInfo correlationInfo, int? ttl = null)
    {
        var key = GetKey(correlationInfo.CorrelationId);
        var json = JsonSerializer.Serialize(correlationInfo, _jsonOptions);
        
        if (ttl.HasValue)
        {
            await _db.StringSetAsync(key, json, TimeSpan.FromSeconds(ttl.Value));
            _logger.LogInformation("Saved correlation {CorrelationId} with TTL {TTL} seconds", 
                correlationInfo.CorrelationId, ttl.Value);
        }
        else
        {
            await _db.StringSetAsync(key, json);
            _logger.LogInformation("Saved correlation {CorrelationId} with no TTL", 
                correlationInfo.CorrelationId);
        }
    }

    /// <inheritdoc />
    public async Task<CorrelationInfo> GetCorrelationAsync(Guid correlationId)
    {
        var key = GetKey(correlationId);
        var value = await _db.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
        {
            _logger.LogWarning("Correlation {CorrelationId} not found", correlationId);
            return null;
        }
            
        var correlationInfo = JsonSerializer.Deserialize<CorrelationInfo>(value!, _jsonOptions);
        _logger.LogInformation("Retrieved correlation {CorrelationId} -> saga {SagaId}", 
            correlationId, correlationInfo.SagaId);
        
        return correlationInfo;
    }
    
    public async Task<ImagenIngestionSagaState> GetSagaStateAsync(Guid sagaId)
    {
        var key = GetKey(sagaId);
        var value = await _db.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
            return null;
            
        return JsonSerializer.Deserialize<ImagenIngestionSagaState>(value!, _jsonOptions);
    }
    

    /// <inheritdoc />
    public async Task DeleteCorrelationAsync(Guid correlationId)
    {
        var key = GetKey(correlationId);
        await _db.KeyDeleteAsync(key);
        _logger.LogInformation("Deleted correlation {CorrelationId}", correlationId);
    }
    
    private string GetKey(Guid correlationId) => $"{_keyPrefix}{correlationId}";
}
