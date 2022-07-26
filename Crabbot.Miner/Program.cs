using Crabbot.Miner;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        ConfigOptions options = configuration
            .GetSection("Config")
            .Get<ConfigOptions>();

        services.AddSingleton(options);
        
        services.AddHostedService<Worker>();
    })
    .UseWindowsService(opts =>
    {
        opts.ServiceName = "Crabbot";
    })
    .Build();

await host.RunAsync();