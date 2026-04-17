using Microsoft.EntityFrameworkCore;
using ModuShop.Catalog.Entities;

namespace ModuShop.Catalog.Data;

public class CatalogDbContext : DbContext
{
	public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
	{
	}

	public DbSet<Category> Categories => Set<Category>();
	public DbSet<Product> Products => Set<Product>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema("catalog");

		modelBuilder.Entity<Category>(entity =>
		{
			entity.ToTable("Categories");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Name)
				.HasMaxLength(200)
				.IsRequired();
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.ToTable("Products");
			entity.HasKey(x => x.Id);

			entity.Property(x => x.Name)
				.HasMaxLength(200)
				.IsRequired();

			entity.Property(x => x.Description)
				.HasMaxLength(1000);

			entity.Property(x => x.Price)
				.HasColumnType("decimal(18,2)");

			entity.HasOne(x => x.Category)
				.WithMany(x => x.Products)
				.HasForeignKey(x => x.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}
}