using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using ReviewService.Infrastructure.DB.UOW;
using ReviewService.Infrastructure.DB.Indexes;
using ReviewService.Infrastructure.DB.Seeding;

namespace ReviewService.Infrastructure.DB.Extension
{
    public static class ServiceCollectionExtensions
    {
        private static bool isGuidSerializerRegistered = false;
        private static readonly object _lock = new object();

        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            ConfigureMongoDbSerialization();

            services.AddScoped<IUnitOfWork>(sp =>
            {
                var mongoDb = sp.GetRequiredService<IMongoDatabase>();
                return new UnitOfWork(mongoDb);
            });

            services.AddSingleton<IIndexCreationService>(sp =>
            {
                var mongoDb = sp.GetRequiredService<IMongoDatabase>();
                return new IndexCreationService(mongoDb);
            });

            services.AddSingleton<ReviewSeeder>(sp =>
            {
                var mongoDb = sp.GetRequiredService<IMongoDatabase>();
                return new ReviewSeeder(mongoDb);
            });

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