using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FigureStore.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Khóa ngoại liên kết đến Category
        public int CategoryId { get; set; }

        // Navigation property
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public Category? Category { get; set; }

    }
}
