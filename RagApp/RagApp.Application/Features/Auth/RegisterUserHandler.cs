namespace RagApp.Application.Features.Auth
{ 
    using MediatR;

    using Microsoft.EntityFrameworkCore;

    using RagApp.Application.Abstractions;
    using RagApp.Application.Persistence;
    using RagApp.Domain.Entities; 
     
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly RagDbContext _db;
        private readonly IPasswordHasher _hasher;

        public RegisterUserHandler(RagDbContext db, IPasswordHasher hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
        { 
            if (await _db.Users.AnyAsync(u => u.Nickname == request.Nickname, ct))
                throw new InvalidOperationException("Този nickname вече е зает.");
             
            if (await _db.Users.AnyAsync(u => u.Email == request.Email, ct))
                throw new InvalidOperationException("Този имейл вече е регистриран.");
             
            var user = new User
            {
                Nickname = request.Nickname,
                Email = request.Email, 
                PasswordHash = _hasher.Hash(request.Password),
                AvatarUrl = request.AvatarUrl,
                Age = request.Age,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Bio = request.Bio,
                Country = request.Country,
                City = request.City
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            return user.Id;
        }
    }
}
