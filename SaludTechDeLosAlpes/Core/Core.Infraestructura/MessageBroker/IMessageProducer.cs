using System.Threading.Tasks;

namespace Core.Infraestructura.MessageBroker
{
    public interface IMessageProducer
    {
        Task<string> SendAsync(string topic, byte[] message);
        Task<string> SendJsonAsync<T>(string topic, T message);
    }
}
