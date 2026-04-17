using Microsoft.Extensions.Hosting;
using Serilog;

namespace ModuShop.Reporting;

public static class ReportingModuleExtensions
{
	public static IHostApplicationBuilder AddReportingModuleServices(
		this IHostApplicationBuilder builder,
		ILogger logger)
	{
		logger.Information("{Module} module services registered", "Reporting");
		return builder;
	}
}