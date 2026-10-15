namespace RagApp.Application.Features.Profile
{
    using MediatR;
    using RagApp.Application.Dtos;
     
    public record UpdateProfileCommand(
        Guid UserId,
        string? Nickname,
        string? AvatarUrl,
        int? Age,
        string? FirstName,
        string? LastName,
        string? Bio,
        string? Country,
        string? City) : IRequest<UserProfileDto>;
}
