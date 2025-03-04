using FigureStore.Data;
using FigureStore.Dtos;
using FigureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FigureStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng" });
            }

            var cartItemsDto = cart.CartItems.Select(ci => new
            {
                id = ci.Id,               // CartItem Id
                quantity = ci.Quantity,   // Số lượng
                                          // Gán name và price ngay tại cấp CartItem để khớp với frontend
                name = ci.Product.Name,
                price = ci.Product.Price,

                // Nếu muốn trả về ảnh, lấy ảnh IsPrimary = true
                image = ci.Product.ProductImages
                            .FirstOrDefault(pi => pi.IsPrimary)?.URL
            }).ToList();

            return Ok(new { cartItems = cartItemsDto });
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem user đã có giỏ hàng chưa
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == cartItemDto.UserId);

            if (cart == null)
            {
                // Nếu chưa có thì tạo mới Cart
                cart = new Cart { UserId = cartItemDto.UserId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == cartItemDto.ProductId);
            if (existingItem != null)
            {
                // Nếu đã có, tăng số lượng
                existingItem.Quantity += cartItemDto.Quantity;
                 _context.Entry(existingItem).Property(x => x.CreateAt).IsModified = false;
            }
            else
            {
                // Nếu chưa có, thêm sản phẩm mới (không cần set CreateAt, UpdateAt vì tự động trong SaveChanges)
                var newCartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity
                };
                cart.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã thêm sản phẩm vào giỏ hàng" });
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateQuantityCartItemDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound(new { message = "Cart item not found." });
            }

            cartItem.Quantity = updateDto.Quantity;
            // Nếu sử dụng IHasTimestamps thì UpdateAt sẽ được tự cập nhật khi SaveChanges (theo override trong AppDbContext)
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart item updated successfully." });
        }

        // Endpoint DELETE để xóa CartItem
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound(new { message = "Cart item not found." });
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart item deleted successfully." });
        }
    }
}
