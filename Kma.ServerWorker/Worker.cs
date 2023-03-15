namespace Kma.ServerWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    // TODO: get config from app settings (IOptions)
    private const string ListenFolder = "C:\\Users\\KentMartinÅsvang\\Repos\\ListenFolder";
    private const string VersionsFolder = "C:\\Users\\KentMartinÅsvang\\Repos\\VersionsFolder";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            if (IsDirectoryEmpty(ListenFolder))
            {
                _logger.LogInformation($"{nameof(ListenFolder)} was empty.");
                // TODO: get delay-time from appSettings
                await Task.Delay(10000, stoppingToken);
                continue;
            }
            
            _logger.LogInformation($"Moving published files to {nameof(VersionsFolder)}");
            var newVersion = GetNewVersionNumber();

            try
            {
                Directory.CreateDirectory(Path.Combine(VersionsFolder, newVersion));
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogError($"Directory at '{VersionsFolder}' Don't exist");
                continue;
            }

            foreach (var filePath in Directory.EnumerateFileSystemEntries(ListenFolder))
            {
                var fileName = Path.GetFileName(filePath); 
                Directory.Move(filePath, Path.Combine(VersionsFolder, newVersion, fileName));
            }
        }
    }

    private string GetNewVersionNumber()
    {
        if (IsDirectoryEmpty(VersionsFolder))
        {
            // no previous versions exist, return starting version
            _logger.LogInformation (
                $"{nameof(VersionsFolder)} was empty, returning start version number"
                );
            return "1000";
        }
        
        var paths = Directory.EnumerateDirectories(VersionsFolder).OrderDescending();
        var directories = paths.Select(Path.GetFileName).ToList();

        var latestVersionNumber = directories.OrderDescending().FirstOrDefault()
                                  ?? throw new NullReferenceException("latestBuildNumber Can't be null");

        var newVersionNumber = int.Parse(latestVersionNumber) + 1;
        return $"{newVersionNumber}";
    }

    private bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }
}