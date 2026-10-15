namespace RagApp.Application.Dtos
{
    public record UserProfileDto(
        Guid Id,
        string Nickname,
        string Email,
        string AvatarUrl,
        int Age,
        string FirstName,
        string LastName,
        string Bio,
        string Country,
        string City,
        DateTime CreatedAt);
}
