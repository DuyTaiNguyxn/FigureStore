using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FigureStore.Models;
using FigureStore.Data;
using FigureStore.Dtos;
using FigureStore.Services;

namespace FigureStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public UsersController(AppDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.Address,
                    u.AvatarURL
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(updateDto.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật các trường được phép sửa: Name, Phone, Address
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                user.Name = updateDto.Name;
            }
            if (!string.IsNullOrWhiteSpace(updateDto.Phone))
            {
                user.Phone = updateDto.Phone;
            }
            // Với Address, nếu updateDto.Address khác null, cập nhật ngay cả khi là chuỗi rỗng
            if (updateDto.Address != null)
            {
                user.Address = updateDto.Address;
            }

            // Nếu có Avatar mới được upload, xử lý cập nhật
            if (updateDto.Avatar != null && updateDto.Avatar.Length > 0)
            {
                var tempFileName = Path.GetRandomFileName();
                var tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await updateDto.Avatar.CopyToAsync(stream);
                }
                var imageUrl = _cloudinaryService.UploadImage(tempFilePath);
                System.IO.File.Delete(tempFilePath);
                user.AvatarURL = imageUrl;
            }

            // Không cập nhật CreateAt
            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(user).Property(x => x.CreateAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == updateDto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { user });
        }

    }
}
