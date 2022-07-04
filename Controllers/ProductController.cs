using JwtAuth.Models;
using Microsoft.AspNetCore.Mvc;
using JwtAuth.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuth.Controller
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _products;
        public ProductController(IProductRepository products)
        {
            _products = products;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("products/add")]
        public async Task<IActionResult> Create([FromBody] Products products)
        {
            await _products.Create(products);
            return Ok("Product Added Successfully");
        }

        [Authorize(Roles ="admin")]
        [HttpGet("products/get")]
        public async Task<IEnumerable<Products>> GetProducts()
        {
            return await _products.GetProducts();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("products/edit/{id}")]
        public async Task<ActionResult<Products>> UpdateProduct(int id, [FromBody] Products product)
        {
            
             var result =await _products.GetById(id); 
             Console.WriteLine(result.Price);
            if (id != product.ProductId)
            {
              return BadRequest("Inappropriate Request");
            }
            
            await _products.update(product);
           
            return Ok("Product Updated Successfully");

        }

    }
}