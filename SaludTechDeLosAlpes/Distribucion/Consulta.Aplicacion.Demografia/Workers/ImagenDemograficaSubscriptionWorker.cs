using System.Text.Json;
using Consulta.Aplicacion.Demografia.Comandos;
using Consulta.Dominio.Eventos;
using Core.Infraestructura.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consulta.Aplicacion.Demografia.Workers;

public class ImagenDemograficaSubscriptionWorker : BackgroundService
{
    private readonly ILogger<ImagenDemograficaSubscriptionWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TOPIC_DEMOGRAFIA = "imagen-demografia";
    private const string SUBSCRIPTION_NAME = "demografia-service";
    private IMessageConsumer? _messageConsumer;
    private AsyncServiceScope? _scope;

    public ImagenDemograficaSubscriptionWorker(
        ILogger<ImagenDemograficaSubscriptionWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _scope = _serviceProvider.CreateAsyncScope();
            _messageConsumer = _scope.Value.ServiceProvider.GetRequiredService<IMessageConsumer>();
            var imagenHandler = _scope.Value.ServiceProvider.GetRequiredService<ImagenDemografiaHandler>();

            _logger.LogInformation("Starting subscription to {Topic} with subscription {Subscription}",
                TOPIC_DEMOGRAFIA, SUBSCRIPTION_NAME);

            await _messageConsumer.StartWithSchemaAsync<ImagenDemografiaEvent>(
                TOPIC_DEMOGRAFIA,
                SUBSCRIPTION_NAME,
                imagenHandler.HandleImagenDemografia
            );

            // Keep the worker running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in subscription worker");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        
        try
        {
            await base.StopAsync(stoppingToken);
            if (_messageConsumer != null)
            {
                try 
                {
                    await _messageConsumer.StopAsync();
                    if (_messageConsumer is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping message consumer");
                }
            }

            if (_scope.HasValue)
            {
                await _scope.Value.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during worker shutdown");
        }
    }
}
