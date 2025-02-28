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
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products) // Cần khai báo ICollection<Product> Products trong model Brand
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc Cascade nếu bạn muốn

            modelBuilder.Entity<Product>()
                .HasOne(p => p.SubCategory)
                .WithMany(s => s.Products) // Cần khai báo ICollection<Product> Products trong model SubCategory
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho SubCategory
            modelBuilder.Entity<SubCategory>()
                .HasOne(s => s.Category)
                .WithMany(c => c.SubCategories) // Đã có khai báo ICollection<SubCategory> SubCategories trong model Category
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Thường là Cascade để khi xóa Category thì các SubCategory liên quan cũng bị xóa

            // Nếu cần cấu hình thêm các thuộc tính decimal của Product:
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.CostPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Weight)
                .HasColumnType("decimal(18,2)");
        }
    }
}
