using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogService.DAL.Context
{
    public static class CatalogSeeder
    {
        public static async Task SeedAsync(CatalogDbContext context)
        {
            // ================= Sizes =================
            if (!await context.Sizes.AnyAsync())
            {
                var sizes = new List<Size>
                {
                    new Size { Name = "S" },
                    new Size { Name = "M" },
                    new Size { Name = "L" },
                    new Size { Name = "XL" }
                };
                await context.Sizes.AddRangeAsync(sizes);
                await context.SaveChangesAsync();
            }

            // ================= Recipients =================
            if (!await context.Recipients.AnyAsync())
            {
                var recipients = new List<Recipient>
                {
                    new Recipient { Name = "For mother" },
                    new Recipient { Name = "For father" },
                    new Recipient { Name = "For wife" },
                    new Recipient { Name = "For husband" },
                    new Recipient { Name = "For friend" },
                    new Recipient { Name = "For girlfriend" },
                    new Recipient { Name = "For boyfriend" },
                    new Recipient { Name = "For colleague" },
                    new Recipient { Name = "For sister" },
                    new Recipient { Name = "For brother" },
                    new Recipient { Name = "For grandmother" },
                    new Recipient { Name = "For grandfather" },
                    new Recipient { Name = "For teacher" },
                    new Recipient { Name = "For boss" },
                    new Recipient { Name = "For child" },
                    new Recipient { Name = "For partner" }
                };
                await context.Recipients.AddRangeAsync(recipients);
                await context.SaveChangesAsync();
            }

            // ================= Flowers =================
            if (!await context.Flowers.AnyAsync())
            {
                var flowers = new List<Flower>
                {
                    new Flower { Name = "Red Rose", Color = "Red", Description = "Classic red rose ❤️", Quantity = 100 },
                    new Flower { Name = "White Rose", Color = "White", Description = "Pure white rose 🤍", Quantity = 80 },
                    new Flower { Name = "Pink Rose", Color = "Pink", Description = "Soft pink rose 💗", Quantity = 75 },
                    new Flower { Name = "Yellow Tulip", Color = "Yellow", Description = "Bright yellow tulip 🌼", Quantity = 60 },
                    new Flower { Name = "Red Tulip", Color = "Red", Description = "Red tulip 🔥", Quantity = 50 },
                    new Flower { Name = "Pink Peony", Color = "Pink", Description = "Luxurious peony 🌸", Quantity = 45 },
                    new Flower { Name = "White Chrysanthemum", Color = "White", Description = "Elegant chrysanthemum ❄️", Quantity = 70 },
                    new Flower { Name = "Blue Hydrangea", Color = "Blue", Description = "Full hydrangea 💙", Quantity = 40 },
                    new Flower { Name = "Red Carnation", Color = "Red", Description = "Bright carnation ❤️", Quantity = 90 },
                    new Flower { Name = "White Calla Lily", Color = "White", Description = "Elegant calla lily 🤍", Quantity = 30 },
                    new Flower { Name = "Purple Orchid", Color = "Purple", Description = "Exotic orchid 💜", Quantity = 25 },
                    new Flower { Name = "Black Rose", Color = "Black", Description = "Unique black rose 🖤", Quantity = 20 }
                };
                await context.Flowers.AddRangeAsync(flowers);
                await context.SaveChangesAsync();
            }

            // ================= Events =================
            if (!await context.Events.AnyAsync())
            {
                var events = new List<Event>
                {
                    new Event { Name = "Birthday" },
                    new Event { Name = "Anniversary" },
                    new Event { Name = "Wedding" },
                    new Event { Name = "Graduation" },
                    new Event { Name = "Valentine's Day" },
                    new Event { Name = "Mother's Day" },
                    new Event { Name = "Father's Day" },
                    new Event { Name = "Get Well" },
                    new Event { Name = "Thank You" },
                    new Event { Name = "Sympathy" },
                    new Event { Name = "Housewarming" },
                    new Event { Name = "Promotion" },
                    new Event { Name = "New Baby" }
                };
                await context.Events.AddRangeAsync(events);
                await context.SaveChangesAsync();
            }

            // ================= Bouquets =================
            if (!await context.Bouquets.AnyAsync())
            {
                var sSize = await context.Sizes.FirstAsync(s => s.Name == "S");
                var mSize = await context.Sizes.FirstAsync(s => s.Name == "M");
                var lSize = await context.Sizes.FirstAsync(s => s.Name == "L");

                var mother = await context.Recipients.FirstAsync(r => r.Name == "For mother");
                var father = await context.Recipients.FirstAsync(r => r.Name == "For father");
                var friend = await context.Recipients.FirstAsync(r => r.Name == "For friend");

                var redRose = await context.Flowers.FirstAsync(f => f.Name == "Red Rose");
                var whiteRose = await context.Flowers.FirstAsync(f => f.Name == "White Rose");
                var pinkPeony = await context.Flowers.FirstAsync(f => f.Name == "Pink Peony");

                var birthday = await context.Events.FirstAsync(e => e.Name == "Birthday");
                var anniversary = await context.Events.FirstAsync(e => e.Name == "Anniversary");

                var bouquets = new List<Bouquet>
                {
                    new Bouquet
                    {
                        Name = "Romantic Bouquet",
                        Description = "For romantic occasions",
                        MainPhotoUrl = "https://example.com/romantic.jpg",
                        BouquetSizes = new List<BouquetSize>
                        {
                            new BouquetSize { Size = sSize, Price = 29.99m },
                            new BouquetSize { Size = mSize, Price = 39.99m },
                            new BouquetSize { Size = lSize, Price = 49.99m }
                        },
                        BouquetFlowers = new List<BouquetFlower>
                        {
                            new BouquetFlower { Flower = redRose, Quantity = 5 },
                            new BouquetFlower { Flower = pinkPeony, Quantity = 3 }
                        },
                        BouquetRecipients = new List<BouquetRecipient>
                        {
                            new BouquetRecipient { Recipient = mother },
                            new BouquetRecipient { Recipient = father }
                        },
                        BouquetEvents = new List<BouquetEvent>
                        {
                            new BouquetEvent { Event = birthday },
                            new BouquetEvent { Event = anniversary }
                        },
                        BouquetImages = new List<BouquetImage>
                        {
                            new BouquetImage { ImageUrl = "https://example.com/romantic1.jpg", Position = 1 },
                            new BouquetImage { ImageUrl = "https://example.com/romantic2.jpg", Position = 2 }
                        }
                    },
                    new Bouquet
                    {
                        Name = "Spring Delight",
                        Description = "Fresh spring bouquet",
                        MainPhotoUrl = "https://example.com/spring.jpg",
                        BouquetSizes = new List<BouquetSize>
                        {
                            new BouquetSize { Size = sSize, Price = 19.99m },
                            new BouquetSize { Size = mSize, Price = 29.99m }
                        },
                        BouquetFlowers = new List<BouquetFlower>
                        {
                            new BouquetFlower { Flower = whiteRose, Quantity = 4 },
                            new BouquetFlower { Flower = pinkPeony, Quantity = 2 }
                        },
                        BouquetRecipients = new List<BouquetRecipient>
                        {
                            new BouquetRecipient { Recipient = friend }
                        },
                        BouquetEvents = new List<BouquetEvent>
                        {
                            new BouquetEvent { Event = birthday }
                        },
                        BouquetImages = new List<BouquetImage>
                        {
                            new BouquetImage { ImageUrl = "https://example.com/spring1.jpg", Position = 1 },
                            new BouquetImage { ImageUrl = "https://example.com/spring2.jpg", Position = 2 }
                        }
                    },
                    new Bouquet
                    {
                        Name = "Elegant Mix",
                        Description = "Elegant mixed flowers",
                        MainPhotoUrl = "https://example.com/elegant.jpg",
                        BouquetSizes = new List<BouquetSize>
                        {
                            new BouquetSize { Size = mSize, Price = 34.99m },
                            new BouquetSize { Size = lSize, Price = 44.99m }
                        },
                        BouquetFlowers = new List<BouquetFlower>
                        {
                            new BouquetFlower { Flower = redRose, Quantity = 3 },
                            new BouquetFlower { Flower = whiteRose, Quantity = 3 },
                            new BouquetFlower { Flower = pinkPeony, Quantity = 2 }
                        },
                        BouquetRecipients = new List<BouquetRecipient>
                        {
                            new BouquetRecipient { Recipient = father }
                        },
                        BouquetEvents = new List<BouquetEvent>
                        {
                            new BouquetEvent { Event = birthday }
                        },
                        BouquetImages = new List<BouquetImage>
                        {
                            new BouquetImage { ImageUrl = "https://example.com/elegant1.jpg", Position = 1 },
                            new BouquetImage { ImageUrl = "https://example.com/elegant2.jpg", Position = 2 }
                        }
                    }
                };

                await context.Bouquets.AddRangeAsync(bouquets);
                await context.SaveChangesAsync();
            }

            if (!await context.BouquetSizeFlowers.AnyAsync())
            {
                var romanticBouquet = await context.Bouquets.FirstAsync(b => b.Name == "Romantic Bouquet");
                var springBouquet = await context.Bouquets.FirstAsync(b => b.Name == "Spring Delight");
                var elegantBouquet = await context.Bouquets.FirstAsync(b => b.Name == "Elegant Mix");

                var sSize = await context.Sizes.FirstAsync(s => s.Name == "S");
                var mSize = await context.Sizes.FirstAsync(s => s.Name == "M");
                var lSize = await context.Sizes.FirstAsync(s => s.Name == "L");

                var redRose = await context.Flowers.FirstAsync(f => f.Name == "Red Rose");
                var whiteRose = await context.Flowers.FirstAsync(f => f.Name == "White Rose");
                var pinkPeony = await context.Flowers.FirstAsync(f => f.Name == "Pink Peony");

                var bouquetSizeFlowers = new List<BouquetSizeFlower>
                {
                    // Romantic Bouquet
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = sSize.Id, FlowerId = redRose.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = sSize.Id, FlowerId = pinkPeony.Id, Quantity = 2 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = mSize.Id, FlowerId = redRose.Id, Quantity = 5 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = mSize.Id, FlowerId = pinkPeony.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = lSize.Id, FlowerId = redRose.Id, Quantity = 7 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = lSize.Id, FlowerId = pinkPeony.Id, Quantity = 5 },

                    // Spring Delight
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = sSize.Id, FlowerId = whiteRose.Id, Quantity = 2 },
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = sSize.Id, FlowerId = pinkPeony.Id, Quantity = 1 },
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = mSize.Id, FlowerId = whiteRose.Id, Quantity = 4 },
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = mSize.Id, FlowerId = pinkPeony.Id, Quantity = 2 },

                    // Elegant Mix
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = mSize.Id, FlowerId = redRose.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = mSize.Id, FlowerId = whiteRose.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = mSize.Id, FlowerId = pinkPeony.Id, Quantity = 2 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = lSize.Id, FlowerId = redRose.Id, Quantity = 4 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = lSize.Id, FlowerId = whiteRose.Id, Quantity = 4 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = lSize.Id, FlowerId = pinkPeony.Id, Quantity = 3 }
                };

                await context.BouquetSizeFlowers.AddRangeAsync(bouquetSizeFlowers);
                await context.SaveChangesAsync();
            }

        }
    }
}
