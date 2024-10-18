using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReStoreAPI.Data;
using ReStoreAPI.Entities;
using Microsoft.EntityFrameworkCore;
using ReStoreAPI.Extensions;
using ReStoreAPI.RequestHelpers;
using System.Text.Json;

namespace ReStoreAPI.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery] ProductParams productParams)
        {
            var query = _context.Products
                .Sort(productParams.OrderBy) //Extension function in ProductExtensions
                .Search(productParams.SearchTerm) //Extension function in ProductExtensions
                .Filter(productParams.Brands, productParams.Types) //Extension function in ProductExtensions
                .AsQueryable();

            var products = await PagedList<Product>.ToPagedList(query, productParams.PageNumber, productParams.PageSize);

            Response.AddPaginationHeader(products.MetaData); //Extension function in HttpExtensions

            return products;
        }

        [HttpGet("{id}")] // api/products/3
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            return product;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var brands = await _context.Products.Select(x => x.Brand).Distinct().ToListAsync();
            var types = await _context.Products.Select(x => x.Type).Distinct().ToListAsync();

            return Ok(new {brands, types});
        }
    }
}
