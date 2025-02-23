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
        private bool _disposed;

        public MessageProducer(IOptions<MessageBrokerSettings> settings)
        {
            _client = new PulsarClientBuilder()
                .ServiceUrl(settings.Value.ServiceUrl)
                .BuildAsync().Result;
            _producers = new ConcurrentDictionary<string, IProducer<byte[]>>();
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
            var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
            return await SendAsync(topic, jsonMessage);
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
