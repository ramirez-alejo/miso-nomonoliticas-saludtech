using Consulta.Aplicacion.Comandos;
using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure message broker
var brokerHost = Environment.GetEnvironmentVariable("MessageBroker:Host") 
                 ?? builder.Configuration.GetValue<string>("MessageBroker:Host") 
                 ?? "localhost";

var brokerPort = Environment.GetEnvironmentVariable("MessageBroker:Port") != null
    ? int.Parse(Environment.GetEnvironmentVariable("MessageBroker:Port"))
    : builder.Configuration.GetValue<int>("MessageBroker:Port", 6650);

// Configure MessageBroker settings
builder.Services.Configure<MessageBrokerSettings>(settings => {
    settings.Host = brokerHost;
    settings.Port = brokerPort;
});
builder.Services.AddSingleton<MessageBrokerSettings>();

// Register message broker services
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddScoped<IMessageConsumer, MessageConsumer>();
builder.Services.AddScoped<ImagenCreadaHandler>();

// Register background worker
builder.Services.AddHostedService<ImagenSubscriptionWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
