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
            var requiredStatuses = new[] { "Pending", "AwaitingPayment", "Completed", "Delivering", "PaymentFailed", "Cancelled" };
            foreach (var statusName in requiredStatuses)
            {
                if (!await context.OrderStatuses.AnyAsync(s => s.Name == statusName))
                {
                    context.OrderStatuses.Add(new OrderStatus
                    {
                        Name = statusName,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();

            // ================= Gifts =================
            var giftsData = new List<Gift>
            {
                new Gift { Name = new Dictionary<string, string> { { "ua", "Подарунковий пакет" }, { "en", "Gift Bag" } }, ImageUrl = "https://placehold.co/400x400?text=Gift+Bag", AvailableCount = 50, Price = 10.99m },
                new Gift { Name = new Dictionary<string, string> { { "ua", "Повітряні кульки" }, { "en", "Balloons" } }, ImageUrl = "https://placehold.co/400x400?text=Balloons", AvailableCount = 100, Price = 5.99m },
                new Gift { Name = new Dictionary<string, string> { { "ua", "Печиво" }, { "en", "Cookies" } }, ImageUrl = "https://placehold.co/400x400?text=Cookies", AvailableCount = 80, Price = 7.99m },
                new Gift { Name = new Dictionary<string, string> { { "ua", "Коробка шоколаду" }, { "en", "Chocolate Box" } }, ImageUrl = "https://placehold.co/400x400?text=Chocolate", AvailableCount = 60, Price = 15.49m },
                new Gift { Name = new Dictionary<string, string> { { "ua", "М'яка іграшка" }, { "en", "Plush Toy" } }, ImageUrl = "https://placehold.co/400x400?text=Toy", AvailableCount = 40, Price = 12.99m }
            };

            // Завантажуємо всі подарунки в пам'ять
            var allGifts = await context.Gifts.ToListAsync();

            foreach (var giftData in giftsData)
            {
                var existing = allGifts.FirstOrDefault(g => g.Name != null && g.Name.TryGetValue("ua", out var name) && name == giftData.Name["ua"]);
                if (existing != null)
                {
                    // Оновлюємо ImageUrl якщо він порожній або містить example.com
                    if (string.IsNullOrEmpty(existing.ImageUrl) || existing.ImageUrl.Contains("example.com"))
                    {
                        existing.ImageUrl = giftData.ImageUrl;
                        context.Update(existing);
                    }
                }
                else
                {
                    await context.Gifts.AddAsync(giftData);
                }
            }
            await context.SaveChangesAsync();

            // ================= Example Orders =================
            if (!await context.Orders.AnyAsync())
            {
                // Завантажуємо всі статуси в пам'ять
                var allStatuses = await context.OrderStatuses.ToListAsync();
                var pendingStatus = allStatuses.First(s => s.Name == "Pending");
                var completedStatus = allStatuses.First(s => s.Name == "Completed");

                // Перезавантажуємо подарунки після збереження
                allGifts = await context.Gifts.ToListAsync();
                var giftBag = allGifts.First(g => g.Name["ua"] == "Подарунковий пакет");
                var balloons = allGifts.First(g => g.Name["ua"] == "Повітряні кульки");

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
                        PickupStoreAddress = PickupStore.Hertsena2A,
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