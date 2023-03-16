namespace Kma.ServerWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly WorkerSettings _appSettings;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _appSettings = LoadSettings(configuration);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var newVersionIsPublished = !IsDirectoryEmpty(_appSettings.Published);
            if (newVersionIsPublished)
            {
                var version = GetNextVersion();
                CreateFolder(version);
                MovePublishedFiles(version);
                CreateSymlink(version);
            }

            await Task.Delay(_appSettings.DelayInMilliSeconds, stoppingToken);
        }
    }

    private static WorkerSettings LoadSettings(IConfiguration configuration)
    {
        WorkerSettings settings = new();
        configuration.GetSection(WorkerSettings.Name).Bind(settings);
        return settings;
    }

    private void CreateSymlink(string version)
    {
        var symlink = Path.Combine(_appSettings.Current, "public");

        try
        {
            Directory.Delete(symlink);
        }
        catch (DirectoryNotFoundException)
        {
        }

        Directory.CreateSymbolicLink(symlink, Path.Combine(_appSettings.Versions, version));
    }

    private void MovePublishedFiles(string version)
    {
        foreach (var file in Directory.EnumerateFileSystemEntries(_appSettings.Published))
        {
            var name = Path.GetFileName(file);
            Directory.Move(file, Path.Combine(_appSettings.Versions, version, name));
        }
    }

    private void CreateFolder(string version)
    {
        Directory.CreateDirectory(Path.Combine(_appSettings.Versions, version));
    }

    private string GetNextVersion()
    {
        var noVersionExist = IsDirectoryEmpty(_appSettings.Versions);
        if (noVersionExist)
        {
            return _appSettings.StartVersion;
        }

        var paths = Directory.EnumerateDirectories(_appSettings.Versions).OrderDescending();
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

public class WorkerSettings
{
    public static readonly string Name = nameof(WorkerSettings);
    
    public string Published { get; set; } = string.Empty;
    public string Versions { get; set; } = string.Empty;
    public string Current { get; set; } = string.Empty;
    public int DelayInMilliSeconds { get; set; } = 0;
    public string StartVersion { get; set; } = string.Empty;
}