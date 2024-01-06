using ServerWorker.Deployment;

// get configuration
var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", optional: false)
    .Build();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<WorkerOptions>(config.GetSection(WorkerOptions.Name));
builder.Services.Configure<Services>(config.GetSection(Services.Name));

var host = builder.Build();
host.Run();