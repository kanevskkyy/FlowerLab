using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;
using ReviewService.Domain.ValueObjects;

namespace ReviewService.Application.Features.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid BouquetId,
        UserInfo User,
        int Rating,
        string Comment
    ) : ICommand<Review>;
}
