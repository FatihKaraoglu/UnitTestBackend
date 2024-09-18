using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using UnitTestBackend.Entity;
using UnitTestBackend.Repository.ProductRepository;

namespace UnitTestBackend.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private const decimal MinimumPrice = 1.00m;

        //Represents the Business-Layer
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _repository.GetByCategoryAsync(category);
        }

        public async Task AddProductAsync(Product product)
        {
            // Business Logic 1: There musnt be a Product with an already exisiting Name!
            var existingProduct = await _repository.GetByNameAsync(product.Name);
            if (existingProduct != null)
                throw new ArgumentException($"A product with the name '{product.Name}' already exists.");

            // Business Logik 2: There musnt be Products with no Price or under the minimum Price
            if (product.Price < MinimumPrice)
                throw new ArgumentException($"Price must be at least {MinimumPrice:C}.");

            await _repository.AddAsync(product);
        }

        public async Task ApplyDiscountToCategoryAsync(string category, decimal discountPercentage)
        {
            if (discountPercentage <= 0 || discountPercentage >= 100)
                throw new ArgumentException("Discount percentage must be greater than 0 and less than 100.");

            var products = await _repository.GetByCategoryAsync(category);

            if (!products.Any())
                throw new ArgumentException($"No products found in category '{category}'.");

            foreach (var product in products)
            {
                decimal discountAmount = product.Price * (discountPercentage / 100);
                decimal newPrice = product.Price - discountAmount;

                // Business Logic 3: Producs that have a discount be applied cant be lower then min Price
                if (newPrice < MinimumPrice)
                    throw new InvalidOperationException($"Discounting '{product.Name}' would reduce the price below the minimum allowed ({MinimumPrice:C}).");

                product.Price = Math.Round(newPrice, 2);
            }

            await _repository.SaveChangesAsync();
        }
    }
}
