using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReviewService.Domain.Entities
{
    public class EventLogRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid EventId { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }

}
