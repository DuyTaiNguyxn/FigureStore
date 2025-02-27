using System.ComponentModel.DataAnnotations;

namespace FigureStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Navigation property: Một Category có nhiều SubCategories
        public ICollection<SubCategory>? SubCategories { get; set; }
    }
}
