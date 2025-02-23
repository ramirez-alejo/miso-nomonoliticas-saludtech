using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pulsar.Client.Api;
using Pulsar.Client.Common;

namespace Core.Infraestructura.MessageBroker
{
    public class MessageConsumer : IMessageConsumer, IAsyncDisposable
    {
        private readonly PulsarClient _client;
        private IConsumer<byte[]>? _consumer;
        private bool _disposed;
        private bool _isRunning;

        public MessageConsumer(IOptions<MessageBrokerSettings> settings)
        {
            _client = new PulsarClientBuilder()
                .ServiceUrl(settings.Value.ServiceUrl)
                .BuildAsync().Result;
        }

        public async Task StartAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler)
        {
            ThrowIfDisposed();

            if (_isRunning)
            {
                throw new InvalidOperationException("Consumer is already running");
            }

            _consumer = await _client.NewConsumer()
                .Topic(topic)
                .SubscriptionName(subscriptionName)
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync();

            _isRunning = true;

            // Start message consumption loop
            _ = Task.Run(async () =>
            {
                while (_isRunning && !_disposed)
                {
                    try
                    {
                        var message = await _consumer.ReceiveAsync();
                        try
                        {
                            var messageData = JsonSerializer.Deserialize<T>(message.Data);
                            if (messageData != null)
                            {
                                await messageHandler(messageData);
                                await _consumer.AcknowledgeAsync(message.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error and acknowledge the message to prevent redelivery
                            Console.WriteLine($"Error processing message: {ex.Message}");
                            await _consumer.AcknowledgeAsync(message.MessageId);
                        }
                    }
                    catch (Exception ex) when (!_disposed)
                    {
                        // Log error but continue processing if not disposed
                        Console.WriteLine($"Error receiving message: {ex.Message}");
                        await Task.Delay(1000); // Wait before retrying
                    }
                }
            });
        }

        public async Task StopAsync()
        {
            ThrowIfDisposed();
            _isRunning = false;
            if (_consumer != null)
            {
                await _consumer.DisposeAsync();
                _consumer = null;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MessageConsumer));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _isRunning = false;
                if (_consumer != null)
                {
                    await _consumer.DisposeAsync();
                    _consumer = null;
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
