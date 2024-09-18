using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestBackend.Data;
using UnitTestBackend.Entity;
using UnitTestBackend.Repository.ProductRepository;
using UnitTestBackend.Services.ProductService;

namespace UnitTestBackend.Tests.RepositoryTests
{
    public class ProductRepositoryTests
    {
        //Important to understand we are not testing the validity of the database itself!
        //We are testing the CRUD-Operations of the Repository! 

        //Mocking Database by InMemoryDataBase
        private async Task<AppDbContext> GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UnitTestBackendDb")
                .Options;

            var context = new AppDbContext(options);

            // Seed data
            if (!await context.Products.AnyAsync())
            {
                context.Products.AddRange(
                    new Product { Id = 1, Name = "Pear Phone 99s", Price = 100.99m, Category = "Electronics" },
                    new Product { Id = 2, Name = "PH Desktop PC", Price = 2000.99m, Category = "Electronics"}
                );
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            using var context = await GetInMemoryDbContext();
            //adding the context to the ProductRepository
            var repository = new ProductRepository(context);

            // Act
            var products = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, products.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectProductTest()
        {
            // Arrange
            using var context = await GetInMemoryDbContext();
            var repository = new ProductRepository(context);

            // Act
            var product = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(product);
            Assert.Equal("Pear Phone 99s", product.Name);
            Assert.Equal(100.99m, product.Price);
        }

        [Theory]
        [InlineData(1, "Product A", 100.99)]
        [InlineData(2, "Product B", 2000.99)]
        public async Task GetByIdAsync_ReturnsCorrectProduct(int id, string name, decimal price)
        {
            // Arrange
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(new Product { Id = id, Name = name, Price = price });

            var service = new ProductService(mockRepo.Object);

            // Act
            var product = await service.GetProductByIdAsync(id);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(name, product.Name);
            Assert.Equal(price, product.Price);
        }


    }



}
