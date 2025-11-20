using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ReviewService.Application.Interfaces.Commands;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReview
{
    public record UpdateReviewWithIdCommand(
        string ReviewId,
        string Comment,
        int Rating
    ) : ICommand<Unit>, ICommand
    {
        public Guid RequesterId { get; set; }
        public string RequesterRole { get; set; } = "Client";
    };
}
