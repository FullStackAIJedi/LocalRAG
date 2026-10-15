namespace RagApp.Api.Requests
{
    public record UpdateProfileRequest(
        string? Nickname,
        string? AvatarUrl,
        int? Age,
        string? FirstName,
        string? LastName,
        string? Bio,
        string? Country,
        string? City);
}
