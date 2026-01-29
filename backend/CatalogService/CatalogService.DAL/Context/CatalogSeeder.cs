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
            if (!await context.Sizes.AnyAsync())
            {
                var sizes = new List<Size>
                {
                    new Size { Name = new Dictionary<string, string> { { "ua", "S" }, { "en", "S" } } },
                    new Size { Name = new Dictionary<string, string> { { "ua", "M" }, { "en", "M" } } },
                    new Size { Name = new Dictionary<string, string> { { "ua", "L" }, { "en", "L" } } },
                    new Size { Name = new Dictionary<string, string> { { "ua", "XL" }, { "en", "XL" } } }
                };
                await context.Sizes.AddRangeAsync(sizes);
                await context.SaveChangesAsync();
            }

            if (!await context.Recipients.AnyAsync())
            {
                var recipients = new List<Recipient>
                {
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для мами" }, { "en", "For mother" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для тата" }, { "en", "For father" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для дружини" }, { "en", "For wife" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для чоловіка" }, { "en", "For husband" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для друга" }, { "en", "For friend" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для дівчини" }, { "en", "For girlfriend" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для хлопця" }, { "en", "For boyfriend" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для колеги" }, { "en", "For colleague" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для сестри" }, { "en", "For sister" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для брата" }, { "en", "For brother" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для бабусі" }, { "en", "For grandmother" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для дідуся" }, { "en", "For grandfather" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для вчителя" }, { "en", "For teacher" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для боса" }, { "en", "For boss" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для дитини" }, { "en", "For child" } } },
                    new Recipient { Name = new Dictionary<string, string> { { "ua", "Для партнера" }, { "en", "For partner" } } }
                };
                await context.Recipients.AddRangeAsync(recipients);
                await context.SaveChangesAsync();
            }

            if (!await context.Flowers.AnyAsync())
            {
                var flowers = new List<Flower>
                {
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Червона троянда" }, { "en", "Red Rose" } }, Quantity = 100 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Біла троянда" }, { "en", "White Rose" } }, Quantity = 80 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Рожева троянда" }, { "en", "Pink Rose" } }, Quantity = 75 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Жовтий тюльпан" }, { "en", "Yellow Tulip" } }, Quantity = 60 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Червоний тюльпан" }, { "en", "Red Tulip" } }, Quantity = 50 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Рожева півонія" }, { "en", "Pink Peony" } }, Quantity = 45 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Біла хризантема" }, { "en", "White Chrysanthemum" } }, Quantity = 70 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Блакитна гортензія" }, { "en", "Blue Hydrangea" } }, Quantity = 40 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Червона гвоздика" }, { "en", "Red Carnation" } }, Quantity = 90 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Біла кала" }, { "en", "White Calla Lily" } }, Quantity = 30 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Фіолетова орхідея" }, { "en", "Purple Orchid" } }, Quantity = 25 },
                    new Flower { Name = new Dictionary<string, string> { { "ua", "Чорна троянда" }, { "en", "Black Rose" } }, Quantity = 20 }
                };
                await context.Flowers.AddRangeAsync(flowers);
                await context.SaveChangesAsync();
            }

            if (!await context.Events.AnyAsync())
            {
                var events = new List<Event>
                {
                    new Event { Name = new Dictionary<string, string> { { "ua", "День народження" }, { "en", "Birthday" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Річниця" }, { "en", "Anniversary" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Весілля" }, { "en", "Wedding" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Випускний" }, { "en", "Graduation" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "День закоханих" }, { "en", "Valentine's Day" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "День матері" }, { "en", "Mother's Day" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "День батька" }, { "en", "Father's Day" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Одужуй" }, { "en", "Get Well" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Дякую" }, { "en", "Thank You" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Співчуття" }, { "en", "Sympathy" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Новосілля" }, { "en", "Housewarming" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Підвищення" }, { "en", "Promotion" } } },
                    new Event { Name = new Dictionary<string, string> { { "ua", "Народження дитини" }, { "en", "New Baby" } } }
                };
                await context.Events.AddRangeAsync(events);
                await context.SaveChangesAsync();
            }

            if (!await context.Bouquets.AnyAsync())
            {
                var sSize = await context.Sizes.FirstAsync(s => s.Name["en"] == "S");
                var mSize = await context.Sizes.FirstAsync(s => s.Name["en"] == "M");
                var lSize = await context.Sizes.FirstAsync(s => s.Name["en"] == "L");

                var mother = await context.Recipients.FirstAsync(r => r.Name["en"] == "For mother");
                var father = await context.Recipients.FirstAsync(r => r.Name["en"] == "For father");
                var friend = await context.Recipients.FirstAsync(r => r.Name["en"] == "For friend");

                var redRose = await context.Flowers.FirstAsync(f => f.Name["en"] == "Red Rose");
                var whiteRose = await context.Flowers.FirstAsync(f => f.Name["en"] == "White Rose");
                var pinkPeony = await context.Flowers.FirstAsync(f => f.Name["en"] == "Pink Peony");

                var birthday = await context.Events.FirstAsync(e => e.Name["en"] == "Birthday");
                var anniversary = await context.Events.FirstAsync(e => e.Name["en"] == "Anniversary");

                var bouquets = new List<Bouquet>
                {
                    new Bouquet
                    {
                        Name = new Dictionary<string, string> { { "ua", "Романтичний букет" }, { "en", "Romantic Bouquet" } },
                        Description = new Dictionary<string, string> { { "ua", "Для романтичних подій" }, { "en", "For romantic occasions" } },
                        MainPhotoUrl = "/images/bouquets/romantic_bouquet_main.png",
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
                        }
                    },

                    new Bouquet
                    {
                        Name = new Dictionary<string, string> { { "ua", "Весняна насолода" }, { "en", "Spring Delight" } },
                        Description = new Dictionary<string, string> { { "ua", "Свіжий весняний букет" }, { "en", "Fresh spring bouquet" } },
                        MainPhotoUrl = "/images/bouquets/spring_delight_main.png",
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
                        }
                    },

                    new Bouquet
                    {
                        Name = new Dictionary<string, string> { { "ua", "Елегантний мікс" }, { "en", "Elegant Mix" } },
                        Description = new Dictionary<string, string> { { "ua", "Елегантний мікс квітів" }, { "en", "Elegant mixed flowers" } },
                        MainPhotoUrl = "/images/bouquets/elegant_mix_main.png",
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
                        }
                    }
                };

                await context.Bouquets.AddRangeAsync(bouquets);
                await context.SaveChangesAsync();
            }

            if (!await context.BouquetSizeFlowers.AnyAsync())
            {
                var romanticBouquet = await context.Bouquets
                    .Include(b => b.BouquetSizes)
                        .ThenInclude(bs => bs.Size)
                    .FirstAsync(b => b.Name["en"] == "Romantic Bouquet");

                var springBouquet = await context.Bouquets
                    .Include(b => b.BouquetSizes)
                        .ThenInclude(bs => bs.Size)
                    .FirstAsync(b => b.Name["en"] == "Spring Delight");

                var elegantBouquet = await context.Bouquets
                    .Include(b => b.BouquetSizes)
                        .ThenInclude(bs => bs.Size)
                    .FirstAsync(b => b.Name["en"] == "Elegant Mix");

                var redRose = await context.Flowers.FirstAsync(f => f.Name["en"] == "Red Rose");
                var whiteRose = await context.Flowers.FirstAsync(f => f.Name["en"] == "White Rose");
                var pinkPeony = await context.Flowers.FirstAsync(f => f.Name["en"] == "Pink Peony");

                var romanticS = romanticBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "S");
                var romanticM = romanticBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "M");
                var romanticL = romanticBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "L");

                var springS = springBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "S");
                var springM = springBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "M");

                var elegantM = elegantBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "M");
                var elegantL = elegantBouquet.BouquetSizes.First(bs => bs.Size.Name["en"] == "L");

                var bouquetSizeFlowers = new List<BouquetSizeFlower>
                {
    
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = romanticS.SizeId, FlowerId = redRose.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = romanticS.SizeId, FlowerId = pinkPeony.Id, Quantity = 2 },
                    

                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = romanticM.SizeId, FlowerId = redRose.Id, Quantity = 5 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = romanticM.SizeId, FlowerId = pinkPeony.Id, Quantity = 3 },
                    
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = romanticL.SizeId, FlowerId = redRose.Id, Quantity = 7 },
                    new BouquetSizeFlower { BouquetId = romanticBouquet.Id, SizeId = romanticL.SizeId, FlowerId = pinkPeony.Id, Quantity = 5 },

                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = springS.SizeId, FlowerId = whiteRose.Id, Quantity = 2 },
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = springS.SizeId, FlowerId = pinkPeony.Id, Quantity = 1 },
                    
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = springM.SizeId, FlowerId = whiteRose.Id, Quantity = 4 },
                    new BouquetSizeFlower { BouquetId = springBouquet.Id, SizeId = springM.SizeId, FlowerId = pinkPeony.Id, Quantity = 2 },

                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = elegantM.SizeId, FlowerId = redRose.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = elegantM.SizeId, FlowerId = whiteRose.Id, Quantity = 3 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = elegantM.SizeId, FlowerId = pinkPeony.Id, Quantity = 2 },
                    
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = elegantL.SizeId, FlowerId = redRose.Id, Quantity = 4 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = elegantL.SizeId, FlowerId = whiteRose.Id, Quantity = 4 },
                    new BouquetSizeFlower { BouquetId = elegantBouquet.Id, SizeId = elegantL.SizeId, FlowerId = pinkPeony.Id, Quantity = 3 }
                };

                await context.BouquetSizeFlowers.AddRangeAsync(bouquetSizeFlowers);
                await context.SaveChangesAsync();
            }
        }
    }
}