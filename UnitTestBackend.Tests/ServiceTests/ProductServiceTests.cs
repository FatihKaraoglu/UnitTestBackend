using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestBackend.Entity;
using UnitTestBackend.Repository.ProductRepository;
using UnitTestBackend.Services.ProductService;

namespace UnitTestBackend.Tests.ServiceTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly IProductService _service;

        //Important to understand!
        //The Business-Logic of the ProductService itself is being tested not the ProductRepository itself!
        public ProductServiceTests()
        {
            //Creating a Mock of the ProductRepository 
            _mockRepo = new Mock<IProductRepository>();

            //Giving the mocked Repo to the Service trough the constructor
            _service = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProduct_WhenValid()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "New Product",
                Price = 15.00m,
                Category = "Electronics"
            };

            _mockRepo.Setup(repo => repo.GetByNameAsync(newProduct.Name))
                     .ReturnsAsync((Product)null);

            // Act
            await _service.AddProductAsync(newProduct);

            // Assert
            //Check if the mockedRepo has added the Product to the database by checking if the addAsync method has been called once
            _mockRepo.Verify(repo => repo.AddAsync(newProduct), Times.Once);
        }

        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenDuplicateName()
        {
            // Arrange
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Existing Product",
                Price = 20.00m,
                Category = "Books"
            };

            var newProduct = new Product
            {
                Name = "Existing Product",
                Price = 25.00m,
                Category = "Books"
            };

            _mockRepo.Setup(repo => repo.GetByNameAsync(newProduct.Name))
                     .ReturnsAsync(existingProduct);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductAsync(newProduct));
            Assert.Equal($"A product with the name '{newProduct.Name}' already exists.", exception.Message);

            //Checks if the mockedRepo called the AddAsync Method. Ensuring the invalid product was never added to the database
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenPriceBelowMinimum()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "Cheap Product",
                Price = 0.50m,
                Category = "Accessories"
            };

            _mockRepo.Setup(repo => repo.GetByNameAsync(newProduct.Name))
                     .ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductAsync(newProduct));
            Assert.Equal("Price must be at least 1,00 €.", exception.Message);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task ApplyDiscountToCategoryAsync_ShouldApplyDiscount_WhenValid()
        {
            // Arrange
            string category = "Electronics";
            decimal discountPercentage = 10.0m;

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1000.00m, Category = category },
                new Product { Id = 2, Name = "Smartphone", Price = 500.00m, Category = category }
            };

            _mockRepo.Setup(repo => repo.GetByCategoryAsync(category))
                     .ReturnsAsync(products);

            // Act
            await _service.ApplyDiscountToCategoryAsync(category, discountPercentage);

            // Assert
            Assert.Equal(900.00m, products[0].Price);
            Assert.Equal(450.00m, products[1].Price);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApplyDiscountToCategoryAsync_ShouldThrowException_WhenDiscountTooHigh()
        {
            // Arrange
            string category = "Electronics";
            decimal discountPercentage = 95.0m; 

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1000.00m, Category = category }, // Price after 95% discount = 50.00
                new Product { Id = 2, Name = "Smartphone", Price = 5.00m, Category = category } // Price after 95% discount = 0.25
            };

            _mockRepo.Setup(repo => repo.GetByCategoryAsync(category))
                     .ReturnsAsync(products);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ApplyDiscountToCategoryAsync(category, discountPercentage));
            Assert.Equal($"Discounting '{products[1].Name}' would reduce the price below the minimum allowed (1,00 €).", exception.Message);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ApplyDiscountToCategoryAsync_ShouldThrowException_WhenNoProductsFound()
        {
            // Arrange
            string category = "NonExistentCategory";
            decimal discountPercentage = 10.0m;

            _mockRepo.Setup(repo => repo.GetByCategoryAsync(category))
                     .ReturnsAsync(new List<Product>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ApplyDiscountToCategoryAsync(category, discountPercentage));
            Assert.Equal($"No products found in category '{category}'.", exception.Message);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ApplyDiscountToCategoryAsync_ShouldThrowException_WhenInvalidDiscountPercentage()
        {
            // Arrange
            string category = "Electronics";
            decimal discountPercentage = 0.0m; // Invalid discount

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.ApplyDiscountToCategoryAsync(category, discountPercentage));
            Assert.Equal("Discount percentage must be greater than 0 and less than 100.", exception.Message);
            _mockRepo.Verify(repo => repo.GetByCategoryAsync(It.IsAny<string>()), Times.Never);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
    }
}
