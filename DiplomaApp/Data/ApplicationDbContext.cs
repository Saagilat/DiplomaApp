using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DiplomaApp.Models;
using DiplomaApp.Core;
using Microsoft.AspNetCore.Identity;

namespace DiplomaApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole[] {
                        new IdentityRole{Id="1",Name=Constants.administrator},
                    });

            modelBuilder.Entity<RequestStatus>()
                .HasData(
                    new RequestStatus[] {
                        new RequestStatus{Id=1, Name="Ожидает", Description=""},
                        new RequestStatus{Id=2, Name="В обработке", Description=""},
                        new RequestStatus{Id=3, Name="Отклонено", Description=""},
                        new RequestStatus{Id=4, Name="Выполнено", Description=""},
                    });

            modelBuilder.Entity<CatalogMap>()
                .Property(e => e.CheckDate).HasColumnType("datetime");
            modelBuilder.Entity<CatalogMap>()
                .HasKey(c => c.MarketplaceId);
            modelBuilder.Entity<CatalogMap>()
                .HasOne(c => c.Marketplace)
                .WithOne(c => c.CatalogMap);

            modelBuilder.Entity<CategoryMap>()
                .HasKey(c => c.MarketplaceId);
            modelBuilder.Entity<CategoryMap>()
                .HasOne(c => c.Marketplace)
                .WithOne(c => c.CategoryMap);

            modelBuilder.Entity<OfferMap>()
                .HasKey(c => c.MarketplaceId);
            modelBuilder.Entity<OfferMap>()
                .HasOne(c => c.Marketplace)
                .WithOne(c => c.OfferMap);


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
            modelBuilder.Entity<Offer>()
                .Property(p => p.CreationDate).HasColumnType("datetime");

            modelBuilder.Entity<OfferPrice>()
                .HasKey(p => new { p.OfferId, p.CheckDate });
            modelBuilder.Entity<OfferPrice>()
                .HasOne(p => p.Offer)
                .WithMany(p => p.OfferPrices)
                .HasForeignKey(p => p.OfferId);
            modelBuilder.Entity<OfferPrice>()
                .Property(p => p.CheckDate).HasColumnType("datetime");


            modelBuilder.Entity<RequestRequestStatus>()
                .HasKey(p => new { p.RequestId, p.RequestStatusId, p.CreationDate });

            modelBuilder.Entity<RequestRequestStatus>()
                .HasOne(p => p.RequestStatus)
                .WithMany(p => p.RequestStatuses)
                .HasForeignKey(p => p.RequestStatusId);

            modelBuilder.Entity<RequestRequestStatus>()
                .HasOne(p => p.Request)
                .WithMany(p => p.RequestStatuses)
                .HasForeignKey(p => p.RequestId);

        }

        
        public DbSet<DiplomaApp.Models.Marketplace>? Marketplace { get; set; }
        public DbSet<DiplomaApp.Models.Category>? Category { get; set; }
        public DbSet<DiplomaApp.Models.Offer>? Offer { get; set; }
        public DbSet<DiplomaApp.Models.OfferPrice>? OfferPrice { get; set; }
        public DbSet<DiplomaApp.Models.CatalogMap>? CatalogMap { get; set; }
        public DbSet<DiplomaApp.Models.CategoryMap>? CategoryMap { get; set; }
        public DbSet<DiplomaApp.Models.OfferMap>? OfferMap { get; set; }
        public DbSet<DiplomaApp.Models.Request>? Request { get; set; }
        public DbSet<DiplomaApp.Models.RequestStatus>? RequestStatus { get; set; }
        public DbSet<DiplomaApp.Models.RequestRequestStatus>? RequestRequestStatus { get; set; }
    }
}