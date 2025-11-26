using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.DAL.EntityConfig;
using OrderService.Domain.Entities;
using OrderService.Domain.EntityConfig;

namespace OrderService.Domain.Database
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<Gift> Gifts => Set<Gift>();
        public DbSet<OrderItemFlower> OrderItemFlowers => Set<OrderItemFlower>();
        public DbSet<OrderGift> OrderGifts => Set<OrderGift>();
        public DbSet<DeliveryInformation> DeliveryInformations => Set<DeliveryInformation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrderStatusConfiguration());
            modelBuilder.ApplyConfiguration(new GiftConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryInformationConfiguration());
            modelBuilder.ApplyConfiguration(new OrderGiftConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemFlowerConfiguration());
        }
    }
}
