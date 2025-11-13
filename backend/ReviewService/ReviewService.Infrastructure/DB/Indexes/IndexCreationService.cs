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
        private MongoDbContext dbContext;

        public IndexCreationService(MongoDbContext context)
        {
            dbContext = context;
        }

        public void CreateIndexes()
        {
            var reviewCollection = dbContext.Reviews;
            var reviewKeys = Builders<Review>.IndexKeys;

            reviewCollection.Indexes.CreateOne(new CreateIndexModel<Review>(reviewKeys.Ascending(r => r.User.UserId)));
            reviewCollection.Indexes.CreateOne(new CreateIndexModel<Review>(reviewKeys.Ascending(r => r.BouquetId)));
            reviewCollection.Indexes.CreateOne(new CreateIndexModel<Review>(reviewKeys.Ascending(r => r.Rating)));
        }
    }
}
