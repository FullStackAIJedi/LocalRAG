namespace RagApp.Api.Controllers
{
    using System.Security.Claims;
 
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RagApp.Application.Features.Ask;

    using MediatR;

    using RagApp.Api.Requests;

    [ApiController]
    [Route("api/ask")]
    [Authorize]
    public class AskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(claim ?? throw new UnauthorizedAccessException());
        }

        // POST /api/ask
        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] AskRequest req)
        {
            // Валидация на въпроса.
            if (string.IsNullOrWhiteSpace(req.Question))
                return BadRequest(new { error = "The question cannot be empty." });

            if (req.Question.Length > 1000)
                return BadRequest(new { error = "The question is too long (maximum 1,000 characters)." });

            try
            {
                var result = await _mediator.Send(new AskQuestionQuery(
                    GetUserId(),
                    req.Question,
                    req.DocumentIds));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
