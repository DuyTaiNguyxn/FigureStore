using Microsoft.EntityFrameworkCore;
using FigureStore.Models;

namespace FigureStore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Brand> Brands { get; set; }
        // Định nghĩa thêm các bảng khác nếu cần
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Nếu cần cấu hình thêm các ràng buộc hoặc relationship qua Fluent API
            modelBuilder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(sc => sc.Category)
                .HasForeignKey(sc => sc.CategoryId);
        }
    }
}
