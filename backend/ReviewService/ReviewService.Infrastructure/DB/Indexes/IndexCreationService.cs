using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReviewService.Domain.Entities;

namespace ReviewService.Infrastructure.DB.Indexes
{
    public class IndexCreationService : IIndexCreationService
    {
        private readonly IMongoDatabase _database;

        public IndexCreationService(IMongoDatabase database)
        {
            _database = database;
        }

        public void CreateIndexes()
        {
            var reviewCollection = _database.GetCollection<Review>("Reviews");
            var reviewKeys = Builders<Review>.IndexKeys;

            reviewCollection.Indexes.CreateOne(new CreateIndexModel<Review>(
                reviewKeys.Ascending(r => r.User.UserId)
            ));

            reviewCollection.Indexes.CreateOne(new CreateIndexModel<Review>(
                reviewKeys.Ascending(r => r.BouquetId)
            ));

            reviewCollection.Indexes.CreateOne(new CreateIndexModel<Review>(
                reviewKeys.Ascending(r => r.Rating)
            ));
        }
    }
}