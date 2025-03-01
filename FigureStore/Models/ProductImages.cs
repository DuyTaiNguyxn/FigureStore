using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FigureStore.Models
{
    public class ProductImage : IHasTimestamps
    {
        public int Id { get; set; }

        // Đường dẫn đến ảnh (có thể là URL trên server hoặc cloud)
        [Required]
        public string URL { get; set; }
        public bool IsPrimary { get; set; }

        // Khóa ngoại liên kết đến Product
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [JsonIgnore]
        public Product? Product { get; set; }

        // Các thuộc tính timestamp
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
