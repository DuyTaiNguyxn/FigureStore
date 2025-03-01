using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FigureStore.Models;
using FigureStore.Data;
using FigureStore.Dtos;
using FigureStore.Services;

namespace FigureStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public ProductsController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.SubCategory)
                .Include(p => p.ProductImages) // Thêm dòng này để load ProductImages
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
                    },
                    // Lấy ảnh có IsPrimary = true nếu có
                    PrimaryImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary)
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
                .Include(p => p.ProductImages)  // Bao gồm bảng ProductImages
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
                    BrandId = p.BrandId,
                    SubCategoryId = p.SubCategoryId,
                    Brand = new { p.Brand.Id, p.Brand.Name },
                    SubCategory = new { p.SubCategory.Id, p.SubCategory.Name, p.SubCategory.CategoryId },
                    Images = p.ProductImages.Select(pi => new { pi.Id, pi.URL, pi.IsPrimary }).ToList()
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
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto productDto)
        {
            // 1. Tạo bản ghi sản phẩm
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                SubCategoryId = productDto.SubCategoryId,
                BrandId = productDto.BrandId,
                Price = productDto.Price,
                CostPrice = productDto.CostPrice,
                StockQuantity = productDto.StockQuantity,
                Scale = string.IsNullOrEmpty(productDto.Scale) ? null : productDto.Scale,
                Weight = productDto.Weight,
                IsPreOrder = productDto.IsPreOrder,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 2. Nếu có ảnh được upload, xử lý upload lên Cloudinary và lưu vào bảng ProductImages
            if (productDto.Images != null && productDto.Images.Count > 0)
            {
                int index = 0;
                foreach (var file in productDto.Images)
                {
                    // Lưu file tạm thời
                    var tempFileName = Path.GetRandomFileName();
                    var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Upload file lên Cloudinary
                    var imageUrl = _cloudinaryService.UploadImage(tempFilePath);

                    // Xóa file tạm nếu cần
                    System.IO.File.Delete(tempFilePath);

                    // Tạo bản ghi ProductImage, đánh dấu ảnh đầu tiên là ảnh chính
                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        URL = imageUrl,
                        IsPrimary = index == 0 // ảnh đầu tiên là ảnh chính
                    };
                    _context.ProductImages.Add(productImage);
                    index++;
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest();
            }

            // Tìm sản phẩm cần cập nhật
            var product = await _context.Products.Include(p => p.ProductImages)
                                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật các trường thông tin
            product.Name = updateDto.Name;
            product.Description = string.IsNullOrEmpty(updateDto.Description) ? null : updateDto.Description;
            product.SubCategoryId = updateDto.SubCategoryId;
            product.BrandId = updateDto.BrandId;
            product.Price = updateDto.Price;
            product.CostPrice = updateDto.CostPrice;
            product.StockQuantity = updateDto.StockQuantity;
            product.Scale = string.IsNullOrEmpty(updateDto.Scale) ? null : updateDto.Scale;
            product.Weight = updateDto.Weight;
            product.IsPreOrder = updateDto.IsPreOrder;

            // Xử lý xóa các ảnh (nếu có)
            if (!string.IsNullOrEmpty(updateDto.ImagesToDelete))
            {
                // Giả sử ImagesToDelete là JSON array chứa các id (ví dụ: [1,3,5])
                var idsToDelete = System.Text.Json.JsonSerializer.Deserialize<List<int>>(updateDto.ImagesToDelete);
                if (idsToDelete != null)
                {
                    var imagesForDeletion = _context.ProductImages.Where(pi => idsToDelete.Contains(pi.Id)).ToList();
                    foreach (var image in imagesForDeletion)
                    {
                        // Xóa ảnh khỏi Cloudinary
                        _cloudinaryService.DeleteImageByUrl(image.URL);
                        // Xóa bản ghi ảnh
                        _context.ProductImages.Remove(image);
                    }
                }
            }

            // Xử lý upload ảnh mới (nếu có)
            if (updateDto.Images != null && updateDto.Images.Count > 0)
            {
                int index = 0;
                foreach (var file in updateDto.Images)
                {
                    var tempFileName = Path.GetRandomFileName();
                    var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Upload file lên Cloudinary
                    var imageUrl = _cloudinaryService.UploadImage(tempFilePath);
                    System.IO.File.Delete(tempFilePath);

                    // Tạo bản ghi ảnh mới, nếu chưa có ảnh chính, ảnh đầu tiên có thể đặt là chính
                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        URL = imageUrl,
                        IsPrimary = (product.ProductImages == null || !product.ProductImages.Any()) && index == 0
                    };
                    _context.ProductImages.Add(productImage);
                    index++;
                }
            }

            // Lưu các thay đổi, không cập nhật trường CreateAt nếu có
            _context.Entry(product).State = EntityState.Modified;
            _context.Entry(product).Property(x => x.CreateAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
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
            // Lấy sản phẩm cần xóa
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách ảnh liên quan đến sản phẩm
            var productImages = _context.ProductImages.Where(pi => pi.ProductId == id).ToList();

            foreach (var image in productImages)
            {
                // Xóa ảnh khỏi Cloudinary (phải có phương thức DeleteImageByUrl trong CloudinaryService)
                _cloudinaryService.DeleteImageByUrl(image.URL);

                // Xóa bản ghi ảnh khỏi database
                _context.ProductImages.Remove(image);
            }

            // Xóa sản phẩm
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
