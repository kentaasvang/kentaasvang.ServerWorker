using Microsoft.Extensions.Options;

namespace ServerWorker.Deployment;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Services _services;
    private readonly WorkerOptions _options;

    public Worker(
        ILogger<Worker> logger, 
        IOptions<WorkerOptions> options, 
        IOptions<Services> services)
    {
        _logger = logger;
        _services = services.Value;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            foreach (var service in _services.ServiceList)
            {
                _logger.LogInformation(service.Name);
            }

            var delay = TimeSpan.FromSeconds(_options.DelayInSeconds);
            await Task.Delay(delay, stoppingToken);
        }
    }
}