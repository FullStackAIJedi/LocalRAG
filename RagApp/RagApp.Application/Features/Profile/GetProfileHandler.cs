namespace RagApp.Application.Features.Profile
{
    using Microsoft.EntityFrameworkCore;

    using MediatR;

    using RagApp.Application.Dtos;
    using RagApp.Application.Persistence;

    public class GetProfileHandler : IRequestHandler<GetProfileQuery, UserProfileDto>
    {
        private readonly RagDbContext _db;

        public GetProfileHandler(RagDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfileDto> Handle(GetProfileQuery request, CancellationToken ct)
        { 
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct)
                ?? throw new KeyNotFoundException("User not found.");
             
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
