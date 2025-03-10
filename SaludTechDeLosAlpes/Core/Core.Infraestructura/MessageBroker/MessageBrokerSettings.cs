namespace Core.Infraestructura.MessageBroker;

public class MessageBrokerSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    
    /// <summary>
    /// Indicates whether to use schema registry for message serialization/deserialization
    /// </summary>
    public bool UseSchemaRegistry { get; set; } = false;


    public string ServiceUrl => $"pulsar://{Host}:{Port}";
}
