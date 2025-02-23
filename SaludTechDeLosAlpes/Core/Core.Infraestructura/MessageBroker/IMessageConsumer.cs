using System;
using System.Threading.Tasks;

namespace Core.Infraestructura.MessageBroker
{
    public interface IMessageConsumer
    {
        Task StartAsync<T>(string topic, string subscriptionName, Func<T, Task> messageHandler);
        Task StopAsync();
    }
}
