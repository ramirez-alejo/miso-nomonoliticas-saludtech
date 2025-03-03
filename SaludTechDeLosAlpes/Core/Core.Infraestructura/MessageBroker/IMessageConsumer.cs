using System;
using System.Threading.Tasks;

namespace Core.Infraestructura.MessageBroker
{
    public interface IMessageConsumer
    {
        Task StartAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler);
        
        /// <summary>
        /// Starts consuming messages from a topic using a JSON schema
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="topic">The topic to consume messages from</param>
        /// <param name="subscriptionName">The subscription name</param>
        /// <param name="messageHandler">The message handler function</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task StartWithSchemaAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler);
        
        Task StopAsync();
    }
}
