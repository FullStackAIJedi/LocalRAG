namespace RagApp.Api.Controllers
{
    using System.Security.Claims;
 
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MediatR;

    using RagApp.Application.Features.Profile;
    using RagApp.Api.Requests;

    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator; 
        public ProfileController(IMediator mediator) => _mediator = mediator;

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(claim ?? throw new UnauthorizedAccessException());
        }

        // GET /api/profile/me 
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            { 
                return Ok(await _mediator.Send(new GetProfileQuery(GetUserId())));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // PUT /api/profile/me 
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest req)
        {
            try
            {
                var profile = await _mediator.Send(new UpdateProfileCommand(
                    GetUserId(),
                    req.Nickname,
                    req.AvatarUrl,
                    req.Age,
                    req.FirstName,
                    req.LastName,
                    req.Bio,
                    req.Country,
                    req.City));

                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
