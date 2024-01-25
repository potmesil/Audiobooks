using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Audiobooks.ImportJob
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = new HostBuilder()
				.ConfigureWebJobs(ConfigureWebJobs)
				.ConfigureLogging(ConfigureLogging);

			builder.Build().Run();
		}

		private static void ConfigureWebJobs(HostBuilderContext context, IWebJobsBuilder webJobsBuilder)
		{
			webJobsBuilder.AddAzureStorageCoreServices();
			webJobsBuilder.AddAzureStorage();
			webJobsBuilder.AddTimers();
		}

		private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
		{
			var instrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
			if (!instrumentationKey.IsNullOrWhiteSpace())
			{
				loggingBuilder.AddApplicationInsights(options => options.InstrumentationKey = instrumentationKey);
			}
		}
	}
}