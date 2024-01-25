using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Audiobooks.WebApi
{
	public static class ApiKeyAuthenticationDefaults
	{
		public const string AuthenticationScheme = "ApiKey";
		public const string HeaderName = "ApiKey";
	}

	public static class ApiKeyAuthenticationExtensions
	{
		public static AuthenticationBuilder SetApiKeyAuthentication(this IServiceCollection services)
		{
			services.ConfigureSwaggerGen(options =>
			{
				options.AddSecurityDefinition(
					name: ApiKeyAuthenticationDefaults.AuthenticationScheme,
					securityScheme: new ApiKeyScheme
					{
						In = "Header",
						Name = ApiKeyAuthenticationDefaults.HeaderName
					});
			});

			return services
				.AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
				.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.AuthenticationScheme, null);
		}
	}

	public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
	{
	}

	public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
	{
		public ApiKeyAuthenticationHandler(
			IOptionsMonitor<ApiKeyAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var apiKey))
			{
				return AuthenticateResult.NoResult();
			}

			if (!await ValidateApiKeyAsync(apiKey))
			{
				return AuthenticateResult.Fail("Invalid API key.");
			}

			return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(Scheme.Name)), Scheme.Name));
		}

		private Task<bool> ValidateApiKeyAsync(string apiKey)
		{
			// TODO: Validate API key
			return Task.FromResult(apiKey == "mrC9WaEoiYSLrHUhQdZrDMmV67i5J01evAY265UqyOyEF2pSffDnSY1kbAZ31R2j");
		}
	}
}