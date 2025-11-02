using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL.Context
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Bouquet> Bouquets { get; set; }
        public DbSet<BouquetImage> BouquetImages { get; set; }
        public DbSet<BouquetFlower> BouquetFlowers { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<BouquetSize> BouquetSizes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<BouquetEvent> BouquetEvents { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<BouquetRecipient> BouquetRecipients { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<BouquetGift> BouquetGifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Підключає всі конфігурації з Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                entity.UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                    entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
}
