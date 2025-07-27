using Microsoft.EntityFrameworkCore;
using ClothingSearch.Api.Models;

namespace ClothingSearch.Api.Data
{
    public class ClothingSearchContext : DbContext
    {
        public ClothingSearchContext(DbContextOptions<ClothingSearchContext> options)
            : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<SearchCache> SearchCaches { get; set; }
        public DbSet<SearchAnalytics> SearchAnalytics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ProviderType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Config).HasDefaultValue("{}");
                
                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserSetting>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.ClothingSize).HasMaxLength(10);
                entity.Property(e => e.ShoeSize).HasMaxLength(10);
                entity.Property(e => e.ShoeSizeSystem).HasMaxLength(5);
                
                entity.HasOne(d => d.Country)
                    .WithMany(p => p.UserSettings)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SearchCache>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.SearchQuery, e.Category, e.StoreId });
                entity.HasIndex(e => e.ExpiresAt);
                entity.Property(e => e.SearchQuery).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Results).HasDefaultValue("[]");
                
                entity.HasOne(d => d.Store)
                    .WithMany(p => p.SearchCaches)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SearchAnalytics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Query);
                entity.HasIndex(e => e.Timestamp);
                entity.Property(e => e.Query).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Croatia", Currency = "EUR", Created = DateTime.UtcNow },
                new Country { Id = 2, Name = "Germany", Currency = "EUR", Created = DateTime.UtcNow },
                new Country { Id = 3, Name = "United States", Currency = "USD", Created = DateTime.UtcNow },
                new Country { Id = 4, Name = "United Kingdom", Currency = "GBP", Created = DateTime.UtcNow }
            );

            modelBuilder.Entity<Store>().HasData(
                new Store 
                { 
                    Id = 1, 
                    Name = "Amazon", 
                    CountryId = 3, 
                    ProviderType = "affiliate", 
                    Config = "{\"affiliateTag\":\"your-tag\",\"region\":\"US\"}", 
                    IsActive = true, 
                    Created = DateTime.UtcNow 
                },
                new Store 
                { 
                    Id = 2, 
                    Name = "Zalando", 
                    CountryId = 2, 
                    ProviderType = "affiliate", 
                    Config = "{\"affiliateId\":\"your-id\",\"region\":\"DE\"}", 
                    IsActive = true, 
                    Created = DateTime.UtcNow 
                },
                new Store 
                { 
                    Id = 3, 
                    Name = "Hervis", 
                    CountryId = 1, 
                    ProviderType = "scraping", 
                    Config = "{\"baseUrl\":\"https://www.hervis.hr\",\"searchPath\":\"/search\"}", 
                    IsActive = true, 
                    Created = DateTime.UtcNow 
                }
            );
        }
    }
}
