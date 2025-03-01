namespace FigureStore.Dtos
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
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

        // Thuộc tính cho file ảnh mới được upload
        public IFormFileCollection? Images { get; set; }

        // Danh sách id ảnh cần xóa (gửi dưới dạng JSON string từ client)
        public string? ImagesToDelete { get; set; }
    }
}
