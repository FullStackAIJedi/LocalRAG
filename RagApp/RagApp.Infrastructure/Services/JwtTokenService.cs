namespace RagApp.Infrastructure.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    using RagApp.Application.Abstractions;
    using RagApp.Application.Persistence; 
  
    public class JwtTokenService : IJwtTokenService
    {
        private readonly RagDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IConfiguration _config;

        public JwtTokenService(RagDbContext db, IPasswordHasher hasher, IConfiguration config)
        {
            _db = db;
            _hasher = hasher;
            _config = config;
        }
         
        public async Task<string> LoginAsync(string email, string password)
        { 
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");
             
            if (!_hasher.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return GenerateToken(user.Id, user.Nickname);
        }
         
        public string GenerateToken(Guid userId, string nickname)
        { 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
             
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
             
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, nickname)
        };
             
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresMinutes"]!)),
                signingCredentials: creds);
             
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
