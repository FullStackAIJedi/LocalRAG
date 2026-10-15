namespace RagApp.Api.Controllers
{ 
 
    using Microsoft.AspNetCore.Mvc;

    using MediatR;

    using RagApp.Application.Abstractions;
    using RagApp.Application.Features.Auth;
    using RagApp.Api.Requests;
     
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _jwt;

        public AuthController(IMediator mediator, IJwtTokenService jwt)
        {
            _mediator = mediator;
            _jwt = jwt;
        }

        // POST /api/auth/register 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            { 
                var id = await _mediator.Send(new RegisterUserCommand(
                    req.Nickname,
                    req.Email,
                    req.Password,
                    req.AvatarUrl,
                    req.Age,
                    req.FirstName,
                    req.LastName,
                    req.Bio,
                    req.Country,
                    req.City));
                 
                return Ok(new { userId = id });
            }
            catch (InvalidOperationException ex)
            { 
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST /api/auth/login 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var token = await _jwt.LoginAsync(req.Email, req.Password);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            { 
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
