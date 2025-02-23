using Consulta.Aplicacion.Comandos;
using Core.Infraestructura.MessageBroker;
using Consulta.Aplicacion.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure message broker
var pulsarHost = Environment.GetEnvironmentVariable("PULSAR_HOST") 
    ?? builder.Configuration.GetValue<string>("MessageBroker:Host") 
    ?? "localhost";

var pulsarPort = Environment.GetEnvironmentVariable("PULSAR_PORT") != null
    ? int.Parse(Environment.GetEnvironmentVariable("PULSAR_PORT"))
    : builder.Configuration.GetValue<int>("MessageBroker:Port", 6650);

// Configure MessageBroker settings
builder.Services.Configure<MessageBrokerSettings>(settings => {
    settings.Host = pulsarHost;
    settings.Port = pulsarPort;
});

// Register message broker services
builder.Services.AddSingleton<IMessageProducer, MessageProducer>();
builder.Services.AddSingleton<MessageConsumer>();
builder.Services.AddSingleton<IMessageConsumer, ImagenCreadaHandler>();

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
