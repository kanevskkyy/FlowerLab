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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ReviewService.Domain.ValueObjects;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews([FromQuery] ReviewQueryParameters queryParams, CancellationToken cancellationToken)
        {
            GetReviewsQuery query = new GetReviewsQuery(queryParams);

            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewById(string id, CancellationToken cancellationToken)
        {
            GetReviewByIdQuery query = new GetReviewByIdQuery(id);

            var result = await mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command, CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var firstName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Unknown";
            var lastName = User.FindFirstValue(ClaimTypes.Surname) ?? "";
            // PhotoUrl може бути відсутнім, якщо ми його ще не додали в UsersService
            var photoUrl = User.FindFirstValue("PhotoUrl") ?? ""; 

            // Створюємо UserInfo і передаємо в команду
            command.User = new UserInfo(userId, firstName, lastName, photoUrl);
            // -------------------------------------------

            var review = await mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [HttpPut("{id}")]
        [Authorize]
        // 1. Назвіть вхідний параметр "requestDto" або залиште "dto", щоб уникнути плутанини
        public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewCommand requestDto, CancellationToken cancellationToken)
        {
            // Отримуємо ID того, хто стукає
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId)) return Unauthorized();
            
            var role = User.FindFirstValue(ClaimTypes.Role);

            // 2. Створюємо внутрішню команду з іншою назвою (наприклад, "mediatorCommand")
            var mediatorCommand = new UpdateReviewWithIdCommand(id, requestDto.Comment, requestDto.Rating)
            {
                RequesterId = userId,
                RequesterRole = role ?? "Client"
            };

            // 3. Відправляємо правильну команду
            await mediator.Send(mediatorCommand, cancellationToken);

            return NoContent();
        }

        [HttpPatch("status/{id}/confirm")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmReview(string id, CancellationToken cancellationToken)
        {
            ConfirmReviewCommand command = new ConfirmReviewCommand(id);
            var result = await mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(string id, CancellationToken cancellationToken)
        {        
            // Отримуємо ID того, хто стукає
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out var userId)) return Unauthorized();
            var role = User.FindFirstValue(ClaimTypes.Role);

            var command = new DeleteReviewCommand(id)
            {
                RequesterId = userId,
                RequesterRole = role ?? "Client"
            };

            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}
