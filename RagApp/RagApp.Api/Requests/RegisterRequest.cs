namespace RagApp.Api.Requests
{
    public record RegisterRequest(
        string Nickname,
        string Email,
        string Password,
        string AvatarUrl,
        int Age,
        string FirstName,
        string LastName,
        string Bio,
        string Country,
        string City);
}
