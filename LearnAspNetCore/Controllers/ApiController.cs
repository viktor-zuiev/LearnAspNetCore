using System.Threading;
using System.Threading.Tasks;
using LearnAspNetCore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LearnAspNetCore.Controllers
{
	[ApiController]
	[Route("api")]
	[ApiVersion("1.0", Deprecated = true)]
	[ApiVersion("2.0")]
	public class ApiController : ControllerBase
	{
		private readonly IFakeRepository _repository;

		public ApiController(IFakeRepository repository)
		{
			_repository = repository;
		}

		[HttpGet("{id}")]
		public IActionResult GetProduct([FromRoute] string id)
		{
			return Ok(new Product { Id = id, Name = "V1" });
		}

		[HttpGet("{id}")]
		[MapToApiVersion("2.0")]
		public IActionResult GetProductV2([FromRoute] string id)
		{
			return Ok(new Product { Id = id, Name = "V2" });
		}

		[HttpGet("")]
		public async Task<IActionResult> Long(CancellationToken token)
		{
			var result = await _repository.GetSomeNumberAsync(token);
			return Ok(result);
		}

		class Product
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}
	}
}
