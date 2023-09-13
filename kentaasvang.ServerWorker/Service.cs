namespace kentaasvang.ServerWorker;

public record Service(
    string? Name,
    string PublishedDirectory,
    string VersionDirectory,
    string CurrentDirectory
);