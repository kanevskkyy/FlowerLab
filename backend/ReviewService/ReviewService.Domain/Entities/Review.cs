using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using ReviewService.Domain.ValueObjects;

namespace ReviewService.Domain.Entities
{
    public class Review : BaseEntity
    {
        [BsonElement("bouquetId")]
        public Guid BouquetId { get; private set; }

        [BsonElement("user")]
        public UserInfo User { get; private set; }

        [BsonElement("rating")]
        public int Rating { get; private set; }

        [BsonElement("comment")]
        public string Comment { get; private set; }

        [BsonElement("status")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;


        private Review()
        {

        }

        public void Confirm()
        {
            Status = ReviewStatus.Confirmed;
            UpdateTimestamp();
        }

        public Review(Guid bouquetId, UserInfo user, int rating, string comment)
        {
            BouquetId = bouquetId;
            User = user;
            Rating = rating;
            Comment = comment.Trim();
            Status = ReviewStatus.Pending;
        }

        public void UpdateComment(string newComment, int newRating)
        {
            Comment = newComment.Trim();
            Rating = newRating;
            UpdateTimestamp();
        }
    }
}
