using System.ComponentModel.DataAnnotations;

namespace FigureStore.Models
{
    public class User : IHasTimestamps
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        // Lưu ý: Password nên được lưu dưới dạng hash
        [Required]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        public string? Address { get; set; }

        public string? AvatarURL { get; set; }

        [Required]
        public string Role { get; set; }

        public bool IsActive { get; set; }
        // Các thuộc tính timestamp
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
