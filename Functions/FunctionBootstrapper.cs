using Core.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Functions;

public static class FunctionBootstrapper
{
    public static IServiceProvider ServiceProvider
    {
        get;
    }

    static FunctionBootstrapper()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        // Register Core application
        _ = services.AddBotApplication(configuration);

        ServiceProvider = services.BuildServiceProvider();
    }
}
