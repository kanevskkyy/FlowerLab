using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReviewService.Domain.Entities;
using ReviewService.Domain.ValueObjects;

namespace ReviewService.Infrastructure.DB.Seeding
{
    public class ReviewSeeder : IDataSeeder
    {
        private readonly IMongoCollection<Review> _reviews;

        public ReviewSeeder(IMongoDatabase database)
        {
            _reviews = database.GetCollection<Review>("Reviews");
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var existingCount = await _reviews.CountDocumentsAsync(
                FilterDefinition<Review>.Empty,
                cancellationToken: cancellationToken
            );

            if (existingCount > 0) return;

            List<Review> fakeData = new List<Review>
            {
                new Review(
                    Guid.NewGuid(),
                    new UserInfo(Guid.NewGuid(), "Alice", "Smith", "https://randomuser.me/api/portraits/women/1.jpg"),
                    5,
                    "Absolutely loved this bouquet! Fresh flowers and lovely arrangement."
                ),
                new Review(
                    Guid.NewGuid(),
                    new UserInfo(Guid.NewGuid(), "Bob", "Johnson", "https://randomuser.me/api/portraits/men/2.jpg"),
                    4,
                    "Bouquet was nice but delivery was slightly delayed."
                ),
                new Review(
                    Guid.NewGuid(),
                    new UserInfo(Guid.NewGuid(), "Clara", "Davis", "https://randomuser.me/api/portraits/women/3.jpg"),
                    3,
                    "Flowers were okay, not very impressive."
                ),
                new Review(
                    Guid.NewGuid(),
                    new UserInfo(Guid.NewGuid(), "David", "Brown", "https://randomuser.me/api/portraits/men/4.jpg"),
                    5,
                    "Beautiful arrangement! Would order again."
                ),
                new Review(
                    Guid.NewGuid(),
                    new UserInfo(Guid.NewGuid(), "Eva", "Miller", "https://randomuser.me/api/portraits/women/5.jpg"),
                    2,
                    "Bouquet arrived damaged, not satisfied."
                ),
                new Review(
                    Guid.NewGuid(),
                    new UserInfo(Guid.NewGuid(), "Frank", "Wilson", "https://randomuser.me/api/portraits/men/6.jpg"),
                    4,
                    "Lovely flowers, good value for the price."
                )
            };

            var rnd = new Random();
            foreach (var review in fakeData)
            {
                if (rnd.NextDouble() > 0.5)
                    review.Confirm();
            }

            await _reviews.InsertManyAsync(fakeData, cancellationToken: cancellationToken);
        }
    }
}