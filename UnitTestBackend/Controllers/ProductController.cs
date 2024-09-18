using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTestBackend.Entity;
using UnitTestBackend.Helper;
using UnitTestBackend.Repository.ProductRepository;
using UnitTestBackend.Services.ProductService;

namespace UnitTestBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetAllProducts()
        {

            ServiceResponse<List<Product>> response = new ServiceResponse<List<Product>>();
            try
            {
                var products = await _productService.GetAllProductsAsync();

                response.Success = true;
                response.Message = "";
                response.Data = products.ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }


        [HttpPost("GetProductsByCategory")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string category)
        {

            ServiceResponse<List<Product>> response = new ServiceResponse<List<Product>>();
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(category); 

                response.Success = true;
                response.Message = "";
                response.Data = products.ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }


        [HttpPost("GetProduct")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int id)
        {

            ServiceResponse<Product> response = new ServiceResponse<Product>();
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                response.Success = true;
                response.Message = "";
                response.Data = product;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }


        [HttpPost("AddProduct")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddProduct(Product product)
        {

            ServiceResponse<bool> response = new ServiceResponse<bool>();
            try
            {
                await _productService.AddProductAsync(product);

                response.Success = true;
                response.Message = "";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpPost("ApplyDiscountToProducts")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> ApplyDiscountToProducts(string category, decimal discountPercentage)
        {
            ServiceResponse<List<Product>> response = new ServiceResponse<List<Product>>();
            try
            {
                await _productService.ApplyDiscountToCategoryAsync(category, discountPercentage);

                var products = await _productService.GetProductsByCategoryAsync(category);

                response.Success = true;
                response.Message = "";
                response.Data = products.ToList();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }




    }
}
