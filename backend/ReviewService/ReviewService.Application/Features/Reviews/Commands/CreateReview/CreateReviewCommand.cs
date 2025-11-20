using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.ValueObjects;
using System.Text.Json.Serialization;
namespace ReviewService.Application.Features.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid BouquetId,
        int Rating,
        string Comment
    ) : ICommand<Review>
    {
        // Це поле заповнить Контролер з токена. Клієнт його не бачить.
        [JsonIgnore]
        public UserInfo? User { get; set; }
    }
}
