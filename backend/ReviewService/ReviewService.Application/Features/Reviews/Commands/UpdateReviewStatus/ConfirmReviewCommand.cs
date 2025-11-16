using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewService.Application.Interfaces.Commands;
using ReviewService.Domain.Entities;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReviewStatus
{
    public record ConfirmReviewCommand(string ReviewId) : ICommand<Review>;
}
