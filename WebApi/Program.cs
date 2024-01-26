using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Audiobooks.WebApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebHost.CreateDefaultBuilder(args)
				.UseApplicationInsights()
				.ConfigureAppConfiguration((context, config) =>
				{
					config.AddJsonFile(
						path: Path.Combine(context.HostingEnvironment.ContentRootPath, "..", "Shared", "AppSettings.json"),
						optional: true,
						reloadOnChange: true);
				})
				.UseStartup<Startup>()
				.Build()
				.Run();
		}
	}
}