namespace Kma.ServerWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private const string Published = "C:\\Users\\KentMartinÅsvang\\Repos\\ListenFolder";
    private const string VersionsFolder = "C:\\Users\\KentMartinÅsvang\\Repos\\VersionsFolder";
    private readonly int _delayInMilliSeconds = TimeSpan.FromSeconds(5).Milliseconds;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            if (IsDirectoryEmpty(Published))
            {
                _logger.LogInformation($"{nameof(Published)} was empty.");
                await Task.Delay(_delayInMilliSeconds, stoppingToken);
                continue;
            }
            
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

            foreach (var filePath in Directory.EnumerateFileSystemEntries(Published))
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