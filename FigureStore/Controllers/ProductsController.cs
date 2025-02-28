using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FigureStore.Models;
using FigureStore.Data;  // Giả sử DbContext của bạn có tên là AppDbContext

namespace FigureStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.SubCategory)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.CostPrice,
                    p.StockQuantity,
                    p.Scale,
                    p.Weight,
                    p.IsPreOrder,
                    Brand = new
                    {
                        p.Brand.Id,
                        p.Brand.Name
                    },
                    SubCategory = new
                    {
                        p.SubCategory.Id,
                        p.SubCategory.Name,
                        p.SubCategory.CategoryId
                    }
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.SubCategory)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.CostPrice,
                    p.StockQuantity,
                    p.Scale,
                    p.Weight,
                    p.IsPreOrder,
                    BrandId = p.BrandId,         // Thêm trường này
                    SubCategoryId = p.SubCategoryId, // Thêm trường này
                    Brand = new
                    {
                        p.Brand.Id,
                        p.Brand.Name
                    },
                    SubCategory = new
                    {
                        p.SubCategory.Id,
                        p.SubCategory.Name,
                        p.SubCategory.CategoryId
                    }
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }
    }
}
