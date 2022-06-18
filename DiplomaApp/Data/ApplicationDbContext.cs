using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DiplomaApp.Models;
using DiplomaApp.Core;

namespace DiplomaApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatalogPage>()
                .Property(e => e.CheckDate).HasColumnType("datetime");
            modelBuilder.Entity<CatalogPage>()
                .HasKey(c => c.MarketplaceId);
            modelBuilder.Entity<CatalogPage>()
                .HasOne(c => c.Marketplace)
                .WithOne(c => c.CatalogPage);

            modelBuilder.Entity<CategoryPage>()
                .HasKey(c => c.MarketplaceId);
            modelBuilder.Entity<CategoryPage>()
                .HasOne(c => c.Marketplace)
                .WithOne(c => c.CategoryPage);

            modelBuilder.Entity<OfferPage>()
                .HasKey(c => c.MarketplaceId);
            modelBuilder.Entity<OfferPage>()
                .HasOne(c => c.Marketplace)
                .WithOne(c => c.OfferPage);


            modelBuilder.Entity<Category>()
                .HasOne(p => p.Marketplace)
                .WithMany(p => p.Categories)
                .HasForeignKey(p => p.MarketplaceId);

            modelBuilder.Entity<Category>()
                .Property(p => p.CheckDate).HasColumnType("datetime");

            modelBuilder.Entity<Offer>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Offers)
                .HasForeignKey(c => c.CategoryId);
            modelBuilder.Entity<Offer>()
                .Property(p => p.CheckDate).HasColumnType("datetime");

            modelBuilder.Entity<OfferPrice>()
                .HasKey(p => new { p.OfferId, p.CheckDate });
            modelBuilder.Entity<OfferPrice>()
                .HasOne(p => p.Offer)
                .WithMany(p => p.OfferPrices)
                .HasForeignKey(p => p.OfferId);
            modelBuilder.Entity<OfferPrice>()
                .Property(p => p.CheckDate).HasColumnType("datetime");

        }

        
        public DbSet<DiplomaApp.Models.Marketplace>? Marketplace { get; set; }
        public DbSet<DiplomaApp.Models.Category>? Category { get; set; }
        public DbSet<DiplomaApp.Models.Offer>? Offer { get; set; }
        public DbSet<DiplomaApp.Models.OfferPrice>? OfferPrice { get; set; }
        public DbSet<DiplomaApp.Models.CatalogPage>? CatalogPage { get; set; }
        public DbSet<DiplomaApp.Models.CategoryPage>? CategoryPage { get; set; }
        public DbSet<DiplomaApp.Models.OfferPage>? OfferPage { get; set; }
    }
}