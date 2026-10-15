namespace RagApp.Application.Abstractions
{ 
    public interface IJwtTokenService
    { 
        Task<string> LoginAsync(string email, string password); 
        string GenerateToken(Guid userId, string nickname);
    }
}
