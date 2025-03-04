namespace FigureStore.Models
{
    public class CartItem : IHasTimestamps
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        // Các thuộc tính timestamp
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        // Navigation property
        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }

}
