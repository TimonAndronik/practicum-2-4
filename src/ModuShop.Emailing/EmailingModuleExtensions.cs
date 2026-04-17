using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModuShop.Emailing.Contracts;
using Serilog;

namespace ModuShop.Emailing;

public static class EmailingModuleExtensions
{
    public static IHostApplicationBuilder AddEmailingModuleServices(
        this IHostApplicationBuilder builder,
        ILogger logger)
    {
        builder.Services.AddSingleton<IEmailSender>(_ => new LoggingEmailSender(logger));

        logger.Information("{Module} module services registered", "Emailing");

        return builder;
    }
}