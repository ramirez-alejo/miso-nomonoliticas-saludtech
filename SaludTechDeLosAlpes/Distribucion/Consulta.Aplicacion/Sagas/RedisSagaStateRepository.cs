using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Consulta.Aplicacion.Sagas;

public class RedisSagaStateRepository : ISagaStateRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly string _keyPrefix = "consulta:saga:";
    private readonly string _pendingSetKey = "consulta:saga:pending";
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisSagaStateRepository(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<ImagenConsultaSagaState?> GetStateAsync(Guid sagaId)
    {
        var key = GetKey(sagaId);
        var value = await _db.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
            return null;
            
        return JsonSerializer.Deserialize<ImagenConsultaSagaState>(value!, _jsonOptions);
    }

    public async Task SaveStateAsync(ImagenConsultaSagaState state)
    {
        var key = GetKey(state.SagaId);
        var json = JsonSerializer.Serialize(state, _jsonOptions);
        
        await _db.StringSetAsync(key, json);
        
        // If the state is completed or failed, remove it from the pending set
        if (state.Status == "Completed" || state.Status == "Failed")
        {
            await _db.SetRemoveAsync(_pendingSetKey, state.SagaId.ToString());
        }
        else
        {
            // Otherwise, add it to the pending set
            await _db.SetAddAsync(_pendingSetKey, state.SagaId.ToString());
        }
    }

    public async Task<IEnumerable<ImagenConsultaSagaState>> GetPendingStatesAsync()
    {
        var pendingIds = await _db.SetMembersAsync(_pendingSetKey);
        
        if (pendingIds.Length == 0)
            return Enumerable.Empty<ImagenConsultaSagaState>();
            
        var tasks = pendingIds.Select(id => GetStateAsync(Guid.Parse(id!)));
        var states = await Task.WhenAll(tasks);
        
        return states.Where(s => s != null && s.Status != "Completed" && s.Status != "Failed")!;
    }

    public async Task DeleteStateAsync(Guid sagaId)
    {
        var key = GetKey(sagaId);
        await _db.KeyDeleteAsync(key);
        await _db.SetRemoveAsync(_pendingSetKey, sagaId.ToString());
    }
    
    private string GetKey(Guid sagaId) => $"{_keyPrefix}{sagaId}";
}
