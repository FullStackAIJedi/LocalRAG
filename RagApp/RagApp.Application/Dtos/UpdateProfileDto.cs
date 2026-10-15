namespace RagApp.Application.Dtos
{
    public record UpdateProfileDto(
        string? Nickname,
        string? AvatarUrl,
        int? Age,
        string? FirstName,
        string? LastName,
        string? Bio,
        string? Country,
        string? City);
}
