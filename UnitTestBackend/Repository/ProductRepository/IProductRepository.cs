using UnitTestBackend.Entity;

namespace UnitTestBackend.Repository.ProductRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByNameAsync(string name);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
