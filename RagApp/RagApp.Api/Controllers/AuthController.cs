//namespace RagApp.Api.Controllers
//{ 

//    using Microsoft.AspNetCore.Mvc;

//    using MediatR;

//    using RagApp.Application.Abstractions;
//    using RagApp.Application.Features.Auth;
//    using RagApp.Api.Requests;

//    [ApiController]
//    [Route("api/auth")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IMediator _mediator;
//        private readonly IJwtTokenService _jwt;

//        public AuthController(IMediator mediator, IJwtTokenService jwt)
//        {
//            _mediator = mediator;
//            _jwt = jwt;
//        }

//        // POST /api/auth/register 
//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
//        {
//            try
//            { 
//                var id = await _mediator.Send(new RegisterUserCommand(
//                    req.Nickname,
//                    req.Email,
//                    req.Password,
//                    req.AvatarUrl,
//                    req.Age,
//                    req.FirstName,
//                    req.LastName,
//                    req.Bio,
//                    req.Country,
//                    req.City));

//                return Ok(new { userId = id });
//            }
//            catch (InvalidOperationException ex)
//            { 
//                return BadRequest(new { error = ex.Message });
//            }
//        }

//        // POST /api/auth/login 
//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest req)
//        {
//            try
//            {
//                var token = await _jwt.LoginAsync(req.Email, req.Password);
//                return Ok(new { token });
//            }
//            catch (UnauthorizedAccessException ex)
//            { 
//                return Unauthorized(new { error = ex.Message });
//            }
//        }
//    }
//}

 
using Microsoft.AspNetCore.Mvc;

using MediatR;

using RagApp.Application.Abstractions;
using RagApp.Application.Features.Auth;
using RagApp.Api.Requests;

namespace RagApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _jwt;
    private readonly IWebHostEnvironment _env;

    public AuthController(IMediator mediator, IJwtTokenService jwt, IWebHostEnvironment env)
    {
        _mediator = mediator;
        _jwt = jwt;
        _env = env;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        try
        {
            var id = await _mediator.Send(new RegisterUserCommand(
                req.Nickname, req.Email, req.Password, req.AvatarUrl,
                req.Age, req.FirstName, req.LastName, req.Bio, req.Country, req.City));

            return Ok(new { userId = id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
     
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        try
        {
            var token = await _jwt.LoginAsync(req.Email, req.Password);
             
            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),  
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(120),
                Path = "/"
            });
             
            return Ok(new { message = "Logged in successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
     
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });

        return Ok(new { message = "Logged out successfully" });
    }
}
 