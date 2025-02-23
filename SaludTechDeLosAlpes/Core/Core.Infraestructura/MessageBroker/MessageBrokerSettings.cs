namespace Core.Infraestructura.MessageBroker;

public class MessageBrokerSettings
{
    public string Host { get; set; }
    public int Port { get; set; }

    public string ServiceUrl => $"pulsar://{Host}:{Port}";
}
