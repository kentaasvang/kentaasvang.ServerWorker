namespace Kma.ServerWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private const string Published = "C:\\Users\\KentMartinÅsvang\\Repos\\ListenFolder";
    private const string Versions = "C:\\Users\\KentMartinÅsvang\\Repos\\VersionsFolder";
    private readonly int _delayInMilliSeconds = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
    private const string StartVersion = "1000";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var newVersionIsPublished = !IsDirectoryEmpty(Published);
            if (newVersionIsPublished)
            {
                var version = GetNextVersion();
                CreateFolder(version);
                MovePublishedFiles(version);
            }
            
            await Task.Delay(_delayInMilliSeconds, stoppingToken);
        }
    }

    private static void MovePublishedFiles(string version)
    {
        foreach (var file in Directory.EnumerateFileSystemEntries(Published))
        {
            var name = Path.GetFileName(file);
            Directory.Move(file, Path.Combine(Versions, version, name));
        }
    }

    private static void CreateFolder(string version)
    {
        Directory.CreateDirectory(Path.Combine(Versions, version));
    }

    private static string GetNextVersion()
    {
        var noVersionExist = IsDirectoryEmpty(Versions);
        if (noVersionExist)
        {
            return StartVersion;
        }

        var paths = Directory.EnumerateDirectories(Versions).OrderDescending();
        var directories = paths.Select(Path.GetFileName).ToList();

        var latestVersionNumber = directories.OrderDescending().FirstOrDefault()
                                  ?? throw new NullReferenceException("latestBuildNumber Can't be null");

        var newVersionNumber = int.Parse(latestVersionNumber) + 1;
        return $"{newVersionNumber}";
    }

    private static bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }
}