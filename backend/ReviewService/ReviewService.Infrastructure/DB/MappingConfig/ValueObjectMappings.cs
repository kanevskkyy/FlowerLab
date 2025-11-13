using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using ReviewService.Domain.Entities;
using ReviewService.Domain.ValueObjects;

namespace ReviewService.Infrastructure.DB.MappingConfig
{
    public static class ValueObjectMappings
    {
        public static void Register()
        {
            BsonClassMap.RegisterClassMap<UserInfo>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(u => new UserInfo(u.UserId, u.FirstName, u.LastName, u.PhotoUrl));
            });

            BsonClassMap.RegisterClassMap<Review>(cm => cm.AutoMap());
        }
    }
}
