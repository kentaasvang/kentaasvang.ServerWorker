namespace kentaasvang.ServerWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly WorkerSettings _settings;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _settings = LoadSettings(configuration);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            foreach (var service in _settings.Services)
            {
                var newVersionIsPublished = !IsDirectoryEmpty(service.Published);
                if (newVersionIsPublished)
                {
                    var version = GetNextVersion(service.Versions);
                    CreateFolder(service.Versions, version);
                    
                    var newVersionFolder = Path.Combine(service.Versions, version);
                    MoveAllFilesInFolder(service.Published, newVersionFolder);
                    CreateSymlink(Path.Combine(service.Current, "public"), newVersionFolder);
                }
            }

            await Task.Delay(_settings.DelayInMilliSeconds, stoppingToken);
        }
    }

    private static WorkerSettings LoadSettings(IConfiguration configuration)
    {
        WorkerSettings settings = new();
        configuration.GetSection(WorkerSettings.Name).Bind(settings);
        return settings;
    }

    private void CreateSymlink(string from, string to)
    {
        try
        {
            Directory.Delete(from);
        }
        catch (DirectoryNotFoundException)
        {
        }
    
        Directory.CreateSymbolicLink(from, to);
    }

    private void MoveAllFilesInFolder(string from, string to)
    {
        foreach (var file in Directory.EnumerateFileSystemEntries(from))
        {
            var name = Path.GetFileName(file);
            Directory.Move(file, Path.Combine(from, to, name));
        }
    }
    
    
    private void CreateFolder(string filePath, string name)
    {
        Directory.CreateDirectory(Path.Combine(filePath, name));
    }

    private string GetNextVersion(string versions)
    {
        var noVersionExist = IsDirectoryEmpty(versions);
        if (noVersionExist)
        {
            return _settings.StartVersion;
        }
    
        var paths = Directory.EnumerateDirectories(versions).OrderDescending();
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