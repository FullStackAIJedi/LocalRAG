namespace RagApp.Application.Features.Profile
{
    using Microsoft.EntityFrameworkCore;

    using MediatR;

    using RagApp.Application.Dtos;
    using RagApp.Application.Persistence;

    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
    {
        private readonly RagDbContext _db;

        public UpdateProfileHandler(RagDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct)
                ?? throw new KeyNotFoundException("User not found.");
             
            if (request.Nickname != null)
            { 
                if (await _db.Users.AnyAsync(u => u.Nickname == request.Nickname && u.Id != request.UserId, ct))
                    throw new InvalidOperationException("This nickname is already taken.");

                user.Nickname = request.Nickname;
            }

            if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl;
            if (request.Age.HasValue) user.Age = request.Age.Value;
            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.Bio != null) user.Bio = request.Bio;
            if (request.Country != null) user.Country = request.Country;
            if (request.City != null) user.City = request.City;

            await _db.SaveChangesAsync(ct);

            return new UserProfileDto(
                user.Id,
                user.Nickname,
                user.Email,
                user.AvatarUrl,
                user.Age,
                user.FirstName,
                user.LastName,
                user.Bio,
                user.Country,
                user.City,
                user.CreatedAt);
        }
    }
}
