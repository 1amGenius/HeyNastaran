using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Core.DependencyInjection;

IHost host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;

        _ = services.AddBotApplication(configuration);
    })
    .Build();

host.Run();
