using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FigureStore.Models
{
    public class Product : IHasTimestamps
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        // Khóa ngoại liên kết đến SubCategory
        [Required]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        [JsonIgnore]
        public SubCategory? SubCategory { get; set; }

        // Khóa ngoại liên kết đến Brand
        [Required]
        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        [JsonIgnore]
        public Brand? Brand { get; set; }

        // Giá bán của sản phẩm
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Giá vốn của sản phẩm
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        // Số lượng tồn kho
        [Required]
        public int StockQuantity { get; set; }

        // Tỷ lệ (scale) của sản phẩm, ví dụ: "1:12" (có thể là chuỗi)
        public string? Scale { get; set; }

        // Khối lượng sản phẩm
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Weight { get; set; }

        // Cho phép đặt trước hay không
        public bool IsPreOrder { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }

        // Các thuộc tính timestamp
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
