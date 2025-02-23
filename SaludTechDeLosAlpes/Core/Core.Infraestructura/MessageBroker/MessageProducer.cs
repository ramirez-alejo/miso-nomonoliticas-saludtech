using System;
using System.Text.Json;
using System.Threading.Tasks;
using Pulsar.Client.Api;

namespace Core.Infraestructura.MessageBroker
{
    public class MessageProducer : IMessageProducer, IAsyncDisposable
    {
        private readonly PulsarClient _client;
        private readonly IProducer<byte[]> _producer;
        private bool _disposed;

        public MessageProducer(string serviceUrl, string topicName)
        {
            _client = new PulsarClientBuilder()
                .ServiceUrl(serviceUrl)
                .BuildAsync().Result;

            _producer = _client.NewProducer()
                .Topic(topicName)
                .CreateAsync()
                .Result;
        }

        public async Task<string> SendAsync(byte[] message)
        {
            ThrowIfDisposed();
            var messageId = await _producer.SendAsync(message);
            return messageId.ToString();
        }

        public async Task<string> SendJsonAsync<T>(T message)
        {
            ThrowIfDisposed();
            var jsonMessage = JsonSerializer.SerializeToUtf8Bytes(message);
            var messageId = await _producer.SendAsync(jsonMessage);
            return messageId.ToString();
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
                if (_producer != null)
                {
                    await _producer.DisposeAsync();
                }
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
