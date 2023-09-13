namespace kentaasvang.ServerWorker;

public class WorkerSettings
{
    public const string Name = nameof(WorkerSettings);
    public List<Service> Services { get; } = new();
    public int DelayInMilliSeconds { get; set; } = 0;
    public string StartVersion { get; set; } = string.Empty;
}