namespace RagApp.Application.Abstractions
{ 
    public interface IChatService
    { 
        Task<string> GenerateAnswerAsync(string question, string context, CancellationToken ct = default);
    }
}
