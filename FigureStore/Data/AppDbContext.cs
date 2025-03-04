using Microsoft.EntityFrameworkCore;
using FigureStore.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FigureStore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cho Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.SubCategory)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho ProductImage
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình cho SubCategory
            modelBuilder.Entity<SubCategory>()
                .HasOne(s => s.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình cho các thuộc tính decimal của Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.CostPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Weight)
                .HasColumnType("decimal(18,2)");

            // (Tuỳ chọn) Cấu hình timestamp cho các entity implement IHasTimestamps
            // Nếu muốn SQL tự gán khi INSERT (mà không dùng code), giữ default constraint:
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IHasTimestamps).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreateAt")
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("UpdateAt")
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");
                }
            }
        }

        // Override SaveChanges để cập nhật timestamp
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IHasTimestamps &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.Entity is IHasTimestamps entity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        // Gán CreateAt bằng giờ địa phương hiện tại
                        entity.CreateAt = DateTime.Now;
                        entry.Property(nameof(entity.CreateAt)).IsModified = true;
                    }
                    // Luôn cập nhật UpdateAt với giờ địa phương
                    entity.UpdateAt = DateTime.Now;
                    entry.Property(nameof(entity.UpdateAt)).IsModified = true;
                }
            }
        }

    }
}
