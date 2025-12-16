using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReviewService.Domain.Entities;
using shared.events.EventService;

namespace ReviewService.Application.Consumers.EventLog
{
    public class EventLogService : IEventLogService
    {
        private readonly IMongoCollection<EventLogRecord> _collection;

        public EventLogService(IMongoDatabase database)
        {
            _collection = database.GetCollection<EventLogRecord>("processed_events");
        }

        public async Task<bool> HasEventProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<EventLogRecord>.Filter.Eq(e => e.EventId, eventId);
            return await _collection.Find(filter).AnyAsync(cancellationToken);
        }

        public async Task MarkEventAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var processed = new EventLogRecord { EventId = eventId };

            await _collection.InsertOneAsync(processed, cancellationToken: cancellationToken);
        }
    }
}
