using System.Threading.Tasks;

namespace Core.Infraestructura.MessageBroker
{
    public interface IMessageProducer
    {
        Task<string> SendAsync(string topic, byte[] message);
        Task<string> SendJsonAsync<T>(string topic, T message);
        
        /// <summary>
        /// Sends a message using a JSON schema
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="topic">The topic to send the message to</param>
        /// <param name="message">The message to send</param>
        /// <returns>The message ID as a string</returns>
        Task<string> SendWithSchemaAsync<T>(string topic, T message);
    }
}
