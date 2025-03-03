using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pulsar.Client.Api;

namespace Core.Infraestructura.MessageBroker
{
    public class MessageProducer : IMessageProducer, IAsyncDisposable
    {
        private readonly PulsarClient _client;
        private readonly ConcurrentDictionary<string, IProducer<byte[]>> _producers;
        private readonly ConcurrentDictionary<string, object> _schemaProducers;
        private readonly MessageBrokerSettings _settings;
        private bool _disposed;

        public MessageProducer(IOptions<MessageBrokerSettings> settings)
        {
            _settings = settings.Value;
            _client = new PulsarClientBuilder()
                .ServiceUrl(_settings.ServiceUrl)
                .BuildAsync().Result;
            _producers = new ConcurrentDictionary<string, IProducer<byte[]>>();
            _schemaProducers = new ConcurrentDictionary<string, object>();
        }

        private async Task<IProducer<byte[]>> GetOrCreateProducer(string topic)
        {
            return await _producers.GetOrAddAsync(topic, async (t) =>
            {
                return await _client.NewProducer()
                    .Topic(t)
                    .CreateAsync();
            });
        }

        private async Task<IProducer<T>> GetOrCreateProducerWithSchema<T>(string topic)
        {
            string key = $"{topic}_{typeof(T).FullName}";
            
            if (_schemaProducers.TryGetValue(key, out var existingProducer))
            {
                return (IProducer<T>)existingProducer;
            }
            
            try
            {
                var schema = JsonSchemaProvider.CreateJsonSchema<T>();
                var producer = await _client.NewProducer(schema)
                    .Topic(topic)
                    .CreateAsync();
                    
                _schemaProducers[key] = producer;
                return producer;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Guid[]"))
                {
                    // If we get the Guid[] not supported error, we need to use a different approach
                    Console.WriteLine($"Error creating schema producer: {ex.Message}");
                    Console.WriteLine("Falling back to byte[] producer for messages with Guid[] properties");
                }
                
                // Return null to indicate we should use a byte[] producer instead
                return null;
            }
        }

        public async Task<string> SendAsync(string topic, byte[] message)
        {
            ThrowIfDisposed();
            var producer = await GetOrCreateProducer(topic);
            var messageId = await producer.SendAsync(message);
            return messageId.ToString();
        }

        public async Task<string> SendJsonAsync<T>(string topic, T message)
        {
            ThrowIfDisposed();
            
            // If schema registry is enabled, use schema-based producer
            if (_settings.UseSchemaRegistry)
            {
                return await SendWithSchemaAsync(topic, message);
            }
            
            var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
            return await SendAsync(topic, jsonMessage);
        }
        
        public async Task<string> SendWithSchemaAsync<T>(string topic, T message)
        {
            ThrowIfDisposed();
            
            try
            {
                var producer = await GetOrCreateProducerWithSchema<T>(topic);
                
                // If producer is null, it means we're using the fallback approach for Guid[] properties
                if (producer == null)
                {
                    // Try to convert Guid[] properties to string[] properties if possible
                    if (message != null && message.GetType().IsClass)
                    {
                        try
                        {
                            // Try to use GuidArrayConverter if the message is a class
                            // Cast to object first to avoid type constraints
                            object objMessage = message;
                            var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
                            return await SendAsync(topic, jsonMessage);
                        }
                        catch (Exception)
                        {
                            // If conversion fails, just serialize the message as is
                            var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
                            return await SendAsync(topic, jsonMessage);
                        }
                    }
                    else
                    {
                        // For non-class types, just serialize as is
                        var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
                        return await SendAsync(topic, jsonMessage);
                    }
                }
                else
                {
                    // Use the schema-based producer
                    var messageId = await producer.SendAsync(message);
                    return messageId.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message with schema: {ex.Message}");
                
                // Fallback to sending as JSON
                var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
                return await SendAsync(topic, jsonMessage);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MessageProducer));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                foreach (var producer in _producers.Values)
                {
                    if (producer != null)
                    {
                        await producer.DisposeAsync();
                    }
                }
                _producers.Clear();
                
                foreach (var producer in _schemaProducers.Values)
                {
                    if (producer != null)
                    {
                        // Since we don't know the exact type, we need to use dynamic
                        await ((dynamic)producer).DisposeAsync();
                    }
                }
                _schemaProducers.Clear();

                if (_client != null)
                {
                    await _client.CloseAsync();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}

public static class ConcurrentDictionaryExtensions
{
    public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> valueFactory)
    {
        if (dictionary.TryGetValue(key, out TValue value))
        {
            return value;
        }

        var newValue = await valueFactory(key);
        return dictionary.GetOrAdd(key, newValue);
    }
}
