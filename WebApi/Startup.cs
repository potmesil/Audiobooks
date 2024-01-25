using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

[assembly: ApiController]

namespace Audiobooks.WebApi
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvcCore(opt => opt.Filters.Add(new AuthorizeFilter()))
				.AddApiExplorer()
				.AddAuthorization()
				.AddFormatterMappings()
				.AddDataAnnotations()
				.AddJsonFormatters()
				.AddCors()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.SetApiKeyAuthentication();

			services.AddScoped<DbConnection>(provider =>
			{
				var dbConnection = new SqlConnection(provider.GetRequiredService<IConfiguration>().GetConnectionString("Database"));
				dbConnection.Open();

				return dbConnection;
			});

			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc(
					name: "v1",
					info: new Info { Title = "Audiobooks API", Version = "v1" });
				options.OperationFilter<SwaggerFilter>();
				options.ParameterFilter<SwaggerFilter>();
			});
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "Audiobooks API v1");
			});
			app.UseAuthentication();
			app.UseMvc();
		}

		private class SwaggerFilter : IOperationFilter, IParameterFilter
		{
			public void Apply(Operation operation, OperationFilterContext context)
			{
				var anonymAttributes = context.MethodInfo.DeclaringType
					.GetCustomAttributes(true)
					.Union(context.MethodInfo.GetCustomAttributes(true))
					.OfType<AllowAnonymousAttribute>();

				if (!anonymAttributes.Any())
				{
					operation.Responses.TryAdd("401", new Response { Description = "Unauthorized" });
					operation.Responses.TryAdd("403", new Response { Description = "Forbidden" });
					operation.Security = new List<IDictionary<string, IEnumerable<string>>>
					{
						new Dictionary<string, IEnumerable<string>> {{ ApiKeyAuthenticationDefaults.AuthenticationScheme, Enumerable.Empty<string>() }}
					};
				}

				operation.Responses.TryAdd("500", new Response { Description = "Server Error" });
			}

			public void Apply(IParameter parameter, ParameterFilterContext context)
			{
				if (parameter is NonBodyParameter nonBodyParameter && context.ApiParameterDescription.ModelMetadata.BinderType == typeof(CommaSeparatedArrayModelBinder))
				{
					nonBodyParameter.CollectionFormat = "csv";
				}
			}
		}
	}
}