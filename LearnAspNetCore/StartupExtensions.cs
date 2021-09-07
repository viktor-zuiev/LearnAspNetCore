using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace LearnAspNetCore
{
	public static class StartupExtensions
	{
		private static readonly Random Jitterer = new Random();
		private static readonly AsyncRetryPolicy<HttpResponseMessage> ErrorRetryPolicy = Policy
			.HandleResult<HttpResponseMessage>(
				msg => (int)msg.StatusCode == 429 || (int)msg.StatusCode >= 500)
			.WaitAndRetryAsync(
				2,
				retryAttempt =>
				{
					Console.WriteLine($"################### ATTEMPT {retryAttempt}");
					return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
						+ TimeSpan.FromMilliseconds(Jitterer.Next(0, 1000));
				});
		private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreakerPolicy =
			Policy.HandleResult<HttpResponseMessage>(msg => (int)msg.StatusCode == 503)
				.CircuitBreakerAsync(2, TimeSpan.FromMinutes(1));

		private static readonly AsyncPolicyWrap<HttpResponseMessage> ErrorWithBreaker =
			CircuitBreakerPolicy.WrapAsync(ErrorRetryPolicy);

		public static void SetupHttpClients(this IServiceCollection services)
		{
			// Http client factory
			services
				.AddHttpClient(HttpClientType.Github, clnt =>
				{
					clnt.BaseAddress = new Uri("https://api.github.com");
					clnt.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
					clnt.DefaultRequestHeaders.Add("User-Agent", "LearnAspNet");
				})
				.AddTransientHttpErrorPolicy(x => x.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(300)));

			var policyRegistry = services.AddPolicyRegistry();
			policyRegistry.Add(PolicyNames.ErrorWithCircuitBreaker, ErrorWithBreaker);
			services
				.AddHttpClient(HttpClientType.Api, clnt =>
				{
					clnt.BaseAddress = new Uri("https://localhost:5001");
					clnt.DefaultRequestHeaders.Add("User-Agent", "LearnAspNet");
				})
				.AddPolicyHandler(ErrorWithBreaker);
		}
	}

	public static class PolicyNames
	{
		public static string ErrorWithCircuitBreaker => "ErrorWithBreaker";
	}

	public static class HttpClientType
	{
		public static string Api => "apiClient";
		public static string Github => "github";
	}

	public static class ApiRountes
	{
		public static string Base => "https://localhost:5001/";
	}
}
