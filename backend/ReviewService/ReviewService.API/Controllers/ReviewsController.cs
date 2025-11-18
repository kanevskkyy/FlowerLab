using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReviewService.Application.Features.Reviews.Commands.CreateReview;
using ReviewService.Application.Features.Reviews.Commands.DeleteReview;
using ReviewService.Application.Features.Reviews.Commands.UpdateReview;
using ReviewService.Application.Features.Reviews.Commands.UpdateReviewStatus;
using ReviewService.Application.Features.Reviews.Query.GerReviewById;
using ReviewService.Application.Features.Reviews.Query.GetReviews;
using ReviewService.Domain.Entities;
using ReviewService.Domain.Entities.QueryParameters;

namespace ReviewService.API.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private IMediator mediator;

        public ReviewsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] ReviewQueryParameters queryParams, CancellationToken cancellationToken)
        {
            GetReviewsQuery query = new GetReviewsQuery(queryParams);

            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(string id, CancellationToken cancellationToken)
        {
            GetReviewByIdQuery query = new GetReviewByIdQuery(id);

            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command, CancellationToken cancellationToken)
        {
            var review = await mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(new UpdateReviewWithIdCommand(id, command.Comment, command.Rating), cancellationToken);

            return NoContent();
        }

        [HttpPatch("status/{id}/confirm")]
        public async Task<IActionResult> ConfirmReview(string id, CancellationToken cancellationToken)
        {
            ConfirmReviewCommand command = new ConfirmReviewCommand(id);
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(string id, CancellationToken cancellationToken)
        {        
            DeleteReviewCommand command = new DeleteReviewCommand(id);

            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}
