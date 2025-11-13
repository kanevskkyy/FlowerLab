using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using ReviewService.Infrastructure.DB.Settings;
using ReviewService.Infrastructure.DB.UOW;
using ReviewService.Infrastructure.DB.Indexes;
using ReviewService.Infrastructure.DB.Seeding;

namespace ReviewService.Infrastructure.DB.Extension
{
    public static class ServiceCollectionExtensions
    {
        private static bool isGuidSerializerRegistered = false;
        private static readonly object _lock = new object();

        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureMongoDbSerialization();

            var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();

            services.AddSingleton(mongoSettings);

            MongoDbContext context = new MongoDbContext(mongoSettings);
            services.AddSingleton(context);

            services.AddScoped<IUnitOfWork>(sp =>
            {
                var mongoDb = sp.GetRequiredService<MongoDbContext>().Database;
                return new UnitOfWork(mongoDb);
            });

            services.AddSingleton<IIndexCreationService, IndexCreationService>();
            services.AddSingleton<ReviewSeeder>();

            return services;
        }


        private static void ConfigureMongoDbSerialization()
        {
            lock (_lock)
            {
                if (!isGuidSerializerRegistered)
                {
                    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                    isGuidSerializerRegistered = true;
                }
            }
        }

        public static void EnsureIndexes(this IServiceProvider serviceProvider)
        {
            var indexService = serviceProvider.GetRequiredService<IIndexCreationService>();
            indexService.CreateIndexes();
        }
    }
}
