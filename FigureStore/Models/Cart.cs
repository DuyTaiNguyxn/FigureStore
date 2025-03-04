namespace FigureStore.Models
{
    public class Cart : IHasTimestamps
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        // Các thuộc tính timestamp
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public ICollection<CartItem>? CartItems { get; set; } = new List<CartItem>();
    }

}
