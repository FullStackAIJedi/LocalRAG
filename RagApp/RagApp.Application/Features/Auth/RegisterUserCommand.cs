namespace RagApp.Application.Features.Auth
{
    using MediatR;
     
    public record RegisterUserCommand(
        string Nickname,
        string Email,
        string Password,
        string AvatarUrl,
        int Age,
        string FirstName,
        string LastName,
        string Bio,
        string Country,
        string City) : IRequest<Guid>;
}
