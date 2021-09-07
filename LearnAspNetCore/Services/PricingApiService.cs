using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Wrap;

namespace LearnAspNetCore.Services
{
	public class PricingApiService : IPricingApiService
	{
		private readonly IHttpClientFactory _clientFactory;
		private readonly IPolicyRegistry<string> _policyRegistry;

		public PricingApiService(IHttpClientFactory clientFactory,
			IPolicyRegistry<string> policyRegistry)
		{
			_clientFactory = clientFactory;
			_policyRegistry = policyRegistry;
		}

		public async Task<PricingDetails> GetPriceForProductAsync(string id, string currency)
		{
			var wrapper = _policyRegistry.Get<AsyncPolicyWrap<HttpResponseMessage>>(PolicyNames.ErrorWithCircuitBreaker);
			if (wrapper.Outer is AsyncCircuitBreakerPolicy<HttpResponseMessage> cb)
			{
				if (cb.CircuitState == CircuitState.Open)
				{
					throw new Exception("Service currently is unavailable");
				}
			}

			var httpClient = _clientFactory.CreateClient(HttpClientType.Api);
			var response = await httpClient.GetAsync($"{ApiRountes.Base}api/polly/price/{id}/{currency}");

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Service currently is unavailable");
			}

			var responseStr = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<PricingDetails>(responseStr);
		}
	}

	public interface IPricingApiService
	{
		Task<PricingDetails> GetPriceForProductAsync(string id, string currency);
	}
}
