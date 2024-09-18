using UnitTestBackend.Entity;

namespace UnitTestBackend.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task ApplyDiscountToCategoryAsync(string category, decimal discountPercentage);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    }
}
