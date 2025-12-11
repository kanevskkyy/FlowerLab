using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Database;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DAL.DbContext
{
    public static class OrderSeeder
    {
        public static async Task SeedAsync(OrderDbContext context)
        {
            // ================= OrderStatuses =================
            if (!await context.OrderStatuses.AnyAsync())
            {
                var statuses = new List<OrderStatus>
                {
                    new OrderStatus { Name = "Pending", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new OrderStatus { Name = "AwaitingPayment", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new OrderStatus { Name = "Completed", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new OrderStatus { Name = "Delivering", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };
                await context.OrderStatuses.AddRangeAsync(statuses);
                await context.SaveChangesAsync();
            }

            // ================= Gifts =================
            if (!await context.Gifts.AnyAsync())
            {
                var gifts = new List<Gift>
                {
                    new Gift { Name = "Gift Bag", ImageUrl = "https://example.com/giftbag.jpg", AvailableCount = 50, Price = 10.99m },
                    new Gift { Name = "Balloons", ImageUrl = "https://example.com/balloons.jpg", AvailableCount = 100, Price = 5.99m },
                    new Gift { Name = "Cookies", ImageUrl = "https://example.com/cookies.jpg", AvailableCount = 80, Price = 7.99m },
                    new Gift { Name = "Chocolate Box", ImageUrl = "https://example.com/chocolate.jpg", AvailableCount = 60, Price = 15.49m },
                    new Gift { Name = "Plush Toy", ImageUrl = "https://example.com/plush.jpg", AvailableCount = 40, Price = 12.99m }
                };
                await context.Gifts.AddRangeAsync(gifts);
                await context.SaveChangesAsync();
            }

            // ================= Example Orders =================
            if (!await context.Orders.AnyAsync())
            {
                var pendingStatus = await context.OrderStatuses.FirstAsync(s => s.Name == "Pending");
                var completedStatus = await context.OrderStatuses.FirstAsync(s => s.Name == "Completed");

                var giftBag = await context.Gifts.FirstAsync(g => g.Name == "Gift Bag");
                var balloons = await context.Gifts.FirstAsync(g => g.Name == "Balloons");

                var orders = new List<Order>
                {
                    new Order
                    {
                        StatusId = pendingStatus.Id,
                        UserFirstName = "Alice",
                        UserLastName = "Smith",
                        IsDelivery = true,
                        TotalPrice = 35.99m,
                        PhoneNumber = "+380501234567",
                        DeliveryInformation = new DeliveryInformation { Address = "Kyiv, Shevchenko Blvd 10" },
                        OrderGifts = new List<OrderGift>
                        {
                            new OrderGift { GiftId = giftBag.Id, Count = 1 },
                            new OrderGift { GiftId = balloons.Id, Count = 3 }
                        }
                    },
                    new Order
                    {
                        StatusId = completedStatus.Id,
                        UserFirstName = "Bob",
                        UserLastName = "Johnson",
                        IsDelivery = false,
                        TotalPrice = 15.99m,
                        PickupStoreAddress = "Lviv, Freedom St. 5",
                        OrderGifts = new List<OrderGift>
                        {
                            new OrderGift { GiftId = balloons.Id, Count = 2 }
                        }
                    }
                };

                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }
        }
    }
}
