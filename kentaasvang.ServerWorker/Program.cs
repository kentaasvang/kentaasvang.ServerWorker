using kentaasvang.ServerWorker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<ServerWorker>(); })
    .Build();

host.Run();