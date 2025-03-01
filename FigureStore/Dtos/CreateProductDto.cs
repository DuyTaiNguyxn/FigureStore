using Microsoft.AspNetCore.Http;

namespace FigureStore.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SubCategoryId { get; set; }
        public int BrandId { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Scale { get; set; }
        public decimal? Weight { get; set; }
        public bool IsPreOrder { get; set; }
        // Thuộc tính để nhận file upload từ form
        public IFormFileCollection? Images { get; set; }
    }
}
