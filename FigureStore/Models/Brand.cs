namespace FigureStore.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FoundedYear { get; set; }
        public string Country { get; set; }
        public string? Website { get; set; }
    }
}
