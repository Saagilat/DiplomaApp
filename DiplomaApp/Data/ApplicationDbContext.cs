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
            modelBuilder.Entity<ApplicationRole>()
            .HasData(
            new ApplicationRole[] {
                new ApplicationRole{Id="1",Name=Constants.administrator, NormalizedName=Constants.administrator}
            });

            modelBuilder.Entity<Marketplace>()
                .Property(e => e.CategoriesCheckDate).HasColumnType("datetime");

            modelBuilder.Entity<Category>()
                .HasOne(p => p.Marketplace)
                .WithMany(p => p.Categories)
                .HasForeignKey(p => p.MarketplaceId);
            modelBuilder.Entity<Category>()
                .Property(p => p.CheckDate).HasColumnType("datetime");
            modelBuilder.Entity<Category>()
                .Property(p => p.OffersCheckDate).HasColumnType("datetime");

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
    }
}