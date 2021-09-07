using System.Threading.Tasks;

namespace LearnAspNetCore.Services
{
	public class ProductService : IProductService
	{
		public Task<ProductDetails> GetProductDetailsAsync(string id)
		{
			return Task.FromResult(new ProductDetails
			{
				Id = id,
				Name = "Mac book pro 2019",
			});
		}
	}

	public class ProductDetails
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Currency { get; set; }
	}

	public interface IProductService
	{
		Task<ProductDetails> GetProductDetailsAsync(string id);
	}
}
