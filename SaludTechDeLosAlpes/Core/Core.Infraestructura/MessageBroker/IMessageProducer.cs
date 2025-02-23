using System.Threading.Tasks;

namespace Core.Infraestructura.MessageBroker
{
    public interface IMessageProducer
    {
        Task<string> SendAsync(byte[] message);
        Task<string> SendJsonAsync<T>(T message);
    }
}
