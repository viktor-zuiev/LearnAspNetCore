using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearnAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnAspNetCore.Controllers
{
	[Route("api/polly/")]
	public class ResilientPollyController : Controller
	{
		private readonly IPricingApiService _apiPricingService;
		private readonly IProductService _productService;

		public ResilientPollyController(IPricingApiService apiPricingService,
			IProductService productService)
		{
			_apiPricingService = apiPricingService;
			_productService = productService;
		}

		[HttpGet("product/{productId}/{currency}")]
		public async Task<IActionResult> GetProduct(
			[FromRoute] string productId,
			[FromRoute] string currency)
		{
			var product = await _productService.GetProductDetailsAsync(productId);
			var price = await _apiPricingService.GetPriceForProductAsync(productId, currency);

			product.Price = price.Price;
			product.Currency = price.Currency;

			return Ok(product);
		}
	}

	[Route("api/polly/")]
	public class PricingController : Controller
	{
		private readonly IPricingService _pricingService;

		public PricingController(IPricingService pricingService, IPricingApiService apiPricingService)
		{
			_pricingService = pricingService;
		}

		// Endpoint for the IPricingService
		[HttpGet("price/{productId}/{currency}")]
		public async Task<IActionResult> GetPrice(
			[FromRoute] string productId,
			[FromRoute] string currency)
		{
			try
			{
				var result = await _pricingService.GetPriceForProductAsync(productId, currency);
				return Ok(result);
			}
			catch
			{
				return StatusCode(503);
			}
		}
	}
}
