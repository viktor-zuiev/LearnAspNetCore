using System;
using System.Threading.Tasks;

namespace LearnAspNetCore.Services
{
	public class PricingService : IPricingService
	{
		private DateTime _recoveryTime = DateTime.UtcNow;
		private static readonly Random _random = new Random();

		public Task<PricingDetails> GetPriceForProductAsync(string id, string currency)
		{
			if (_recoveryTime > DateTime.UtcNow)
			{
				throw new Exception("Something went wrong");
			}

			if (_recoveryTime < DateTime.UtcNow && _random.Next(1, 4) == 1)
			{
				_recoveryTime = DateTime.UtcNow.AddSeconds(30);
			}

			return Task.FromResult(new PricingDetails
			{
				Id = id,
				Currency = currency,
				Price = 10.55m
			});
		}
	}

	public class PricingDetails
	{
		public string Id { get; set; }
		public string Currency { get; set; }
		public decimal Price { get; set; }
	}

	public interface IPricingService
	{
		Task<PricingDetails> GetPriceForProductAsync(string id, string currency);
	}
}
