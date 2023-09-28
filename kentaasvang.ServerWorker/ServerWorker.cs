namespace kentaasvang.ServerWorker;

public class ServerWorker : BackgroundService
{
    // private readonly ILogger<ServerWorker> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            var dateTimeOffset = DateTimeOffset.UtcNow;
            Console.WriteLine($"Worker running at: {dateTimeOffset:yy-mm-dd hh:mm:ss}");
        }
    }
}