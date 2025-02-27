using FigureStore.Data;
using FigureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FigureStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubCategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SubCategories
        [HttpGet]
        public async Task<IActionResult> GetSubCategories()
        {
            var subCategories = await _context.SubCategories
            .Include(sc => sc.Category)
            .Select(sc => new
            {
                sc.Id,
                sc.Name,
                sc.Description,
                sc.CategoryId,
                Category = new
                {
                    sc.Category.Id,
                    sc.Category.Name,
                    sc.Category.Description
                }
            })
            .ToListAsync();

            return Ok(subCategories);
        }


        // GET: api/SubCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategory>> GetSubCategory(int id)
        {
            var subCategory = await _context.SubCategories.Include(sc => sc.Category)
                                                           .FirstOrDefaultAsync(sc => sc.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return subCategory;
        }

        // POST: api/SubCategories
        [HttpPost]
        public async Task<ActionResult<SubCategory>> CreateSubCategory(SubCategory subCategory)
        {
            _context.SubCategories.Add(subCategory);
            await _context.SaveChangesAsync();

            // Trả về 201 Created kèm thông tin location của resource mới tạo
            return CreatedAtAction(nameof(GetSubCategory), new { id = subCategory.Id }, subCategory);
        }

        // PUT: api/SubCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubCategory(int id, SubCategory subCategory)
        {
            if (id != subCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(subCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubCategoryExists(id))
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

        // DELETE: api/SubCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubCategoryExists(int id)
        {
            return _context.SubCategories.Any(e => e.Id == id);
        }
    }
}
