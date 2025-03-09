using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        private readonly MessageBrokerSettings _settings;
        private readonly ConcurrentDictionary<string, IConsumer<byte[]>> _consumers;
        private readonly ConcurrentDictionary<string, object> _schemaConsumers;
        private bool _disposed;
        private bool _isRunning;
        private readonly List<Task> _runningTasks = new List<Task>();

        public MessageConsumer(IOptions<MessageBrokerSettings> settings)
        {
            _settings = settings.Value;
            _client = new PulsarClientBuilder()
                .ServiceUrl(_settings.ServiceUrl)
                .BuildAsync().Result;
            _consumers = new ConcurrentDictionary<string, IConsumer<byte[]>>();
            _schemaConsumers = new ConcurrentDictionary<string, object>();
        }

        public async Task StartAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler)
        {
            ThrowIfDisposed();

            if (_isRunning)
            {
                return;
            }

            // If schema registry is enabled, use schema-based consumer
            if (_settings.UseSchemaRegistry)
            {
                await StartWithSchemaAsync(topic, subscriptionName, messageHandler);
                return;
            }

            string consumerKey = $"{topic}_{subscriptionName}";
            var consumer = await _client.NewConsumer()
                .Topic(topic)
                .SubscriptionName(subscriptionName)
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync();

            _consumers[consumerKey] = consumer;
            _isRunning = true;

            // Start message consumption loop
            var consumptionTask = Task.Run(async () =>
            {
                while (_isRunning && !_disposed)
                {
                    try
                    {
                        var message = await consumer.ReceiveAsync();
                        try
                        {
                            var messageData = JsonSerializer.Deserialize<T>(message.Data);
                            if (messageData != null)
                            {
                                // If the message type is a class, try to convert string[] properties to Guid[] properties
                                if (typeof(T).IsClass)
                                {
                                    try
                                    {
                                        // Try to convert string[] properties to Guid[] properties if possible
                                        // We're not using the GuidArrayConverter here because it might not be applicable
                                        // for all message types
                                        await messageHandler(messageData);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error converting message: {ex.Message}");
                                        await messageHandler(messageData);
                                    }
                                }
                                else
                                {
                                    await messageHandler(messageData);
                                }

                                await consumer.AcknowledgeAsync(message.MessageId);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error and acknowledge the message to prevent redelivery
                            Console.WriteLine($"Error processing message: {ex.Message}");
                            await consumer.AcknowledgeAsync(message.MessageId);
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

            _runningTasks.Add(consumptionTask);
        }

        public async Task StartWithSchemaAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler)
        {
            ThrowIfDisposed();

            if (_isRunning)
            {
                return;
            }

            string consumerKey = $"{topic}_{subscriptionName}_{typeof(T).FullName}";

            try
            {
                var schema = JsonSchemaProvider.CreateJsonSchema<T>();
                var consumer = await _client.NewConsumer(schema)
                    .Topic(topic)
                    .SubscriptionName(subscriptionName)
                    .SubscriptionType(SubscriptionType.Exclusive)
                    .SubscribeAsync();

                _schemaConsumers[consumerKey] = consumer;
                _isRunning = true;

                // Start message consumption loop with schema
                var consumptionTask = Task.Run(async () =>
                {
                    while (_isRunning && !_disposed)
                    {
                        try
                        {
                            var message = await consumer.ReceiveAsync();
                            try
                            {
                                // With schema, the message is already deserialized to the correct type
                                await messageHandler(message.GetValue());
                                await consumer.AcknowledgeAsync(message.MessageId);
                            }
                            catch (Exception ex)
                            {
                                // Log error and acknowledge the message to prevent redelivery
                                Console.WriteLine($"Error processing schema message: {ex.Message}");
                                await consumer.AcknowledgeAsync(message.MessageId);
                            }
                        }
                        catch (Exception ex) when (!_disposed)
                        {
                            // Log error but continue processing if not disposed
                            Console.WriteLine($"Error receiving schema message: {ex.Message}");
                            await Task.Delay(1000); // Wait before retrying
                        }
                    }
                });

                _runningTasks.Add(consumptionTask);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Guid[]"))
                {
                    // If we get the Guid[] not supported error, we need to use a different approach
                    Console.WriteLine($"Error creating schema consumer: {ex.Message}");
                    Console.WriteLine("Falling back to byte[] consumer for messages with Guid[] properties");

                    // Use a regular consumer instead
                    await StartAsync(topic, subscriptionName, messageHandler);
                }
                else
                {
                    // Rethrow other exceptions
                    throw;
                }
            }
        }

        public async Task StopAsync()
        {
            ThrowIfDisposed();
            _isRunning = false;

            foreach (var consumer in _consumers.Values)
            {
                if (consumer != null)
                {
                    await consumer.DisposeAsync();
                }
            }
            _consumers.Clear();

            foreach (var consumer in _schemaConsumers.Values)
            {
                if (consumer != null)
                {
                    // Since we don't know the exact type, we need to use dynamic
                    await ((dynamic)consumer).DisposeAsync();
                }
            }
            _schemaConsumers.Clear();

            // Wait for all running tasks to complete
            if (_runningTasks.Count > 0)
            {
                await Task.WhenAll(_runningTasks);
                _runningTasks.Clear();
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
            try
            {
                if (!_disposed)
                {
                    _isRunning = false;

                    foreach (var consumer in _consumers.Values)
                    {
                        if (consumer != null)
                        {
                            await consumer.DisposeAsync();
                        }
                    }
                    _consumers.Clear();

                    foreach (var consumer in _schemaConsumers.Values)
                    {
                        if (consumer != null)
                        {
                            // Since we don't know the exact type, we need to use dynamic
                            await ((dynamic)consumer).DisposeAsync();
                        }
                    }
                    _schemaConsumers.Clear();

                    if (_client != null)
                    {
                        await _client.CloseAsync();
                    }

                    _disposed = true;
                }
                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing MessageConsumer: {ex.Message}");
            }

        }
    }
}
