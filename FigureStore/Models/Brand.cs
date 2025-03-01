namespace FigureStore.Models
{
    public class Brand : IHasTimestamps
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FoundedYear { get; set; }
        public string Country { get; set; }
        public string? Website { get; set; }
        public ICollection<Product>? Products { get; set; }

        // Các thuộc tính timestamp
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
