using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReviewService.Domain.Entities;
using ReviewService.Infrastructure.DB.Settings;

namespace ReviewService.Infrastructure.DB
{
    public class MongoDbContext
    {
        private IMongoDatabase database;
        public IMongoClient Client { get; }
        public IMongoCollection<Review> Reviews => database.GetCollection<Review>("Reviews");

        public MongoDbContext(MongoDbSettings settings)
        {
            MongoClientSettings mongoClientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            mongoClientSettings.MaxConnectionPoolSize = settings.MaxConnectionPoolSize;
            mongoClientSettings.MinConnectionPoolSize = settings.MinConnectionPoolSize;
            mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.ConnectTimeoutSeconds);
            mongoClientSettings.SocketTimeout = TimeSpan.FromSeconds(settings.SocketTimeoutSeconds);

            Client = new MongoClient(mongoClientSettings);
            database = Client.GetDatabase(settings.DatabaseName);
        }

        public IMongoDatabase Database => database;
    }
}
