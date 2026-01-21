using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Database;

namespace OrderService.DAL.DbContext
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=64819;Database=FlowerShopOrderDb;Username=postgres;Password=postgres");

            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}
