public class Service
{
    public string Name { get; set; } = string.Empty;
    public string PublishDir { get; set; } = string.Empty;
    public string PublicDir { get; set; } = string.Empty;
    public List<string> Ignore { get; set; } = [];
}

public class Services
{
    public static string Name { get; set; } = nameof(Services);
    public List<Service> ServiceList { get; set; } = [];
}