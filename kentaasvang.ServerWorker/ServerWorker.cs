namespace kentaasvang.ServerWorker;

public class ServerWorker : BackgroundService
{
    private readonly ILogger<ServerWorker> _logger;
    private readonly WorkerSettings _settings;

    public ServerWorker(ILogger<ServerWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _settings = LoadSettings(configuration);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_settings.DelayInMilliSeconds, stoppingToken);
            _logger.LogInformation("Starting Worker at: {time:yy-mm-dd hh:mm:ss}", DateTime.UtcNow);

            if (_settings.Services == null) continue;
            
            foreach (var service in _settings.Services)
            {
                _logger.LogInformation("Updating service: {service}", service.Name);
                var newVersionIsPublished = !IsDirectoryEmpty(service.Published);
                if (!newVersionIsPublished) 
                    continue;
                    
                var version = GetNextVersion(service.Versions);
                CreateFolder(service.Versions, version);
                        
                var newVersionFolder = Path.Combine(service.Versions, version);
                MoveAllFilesInFolder(service.Published, newVersionFolder);
                CreateSymlink(Path.Combine(service.Current, "public"), newVersionFolder);
            }
        }
    }

    private static WorkerSettings LoadSettings(IConfiguration configuration)
    {
        WorkerSettings settings = new();
        configuration.GetSection(nameof(WorkerSettings)).Bind(settings);
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