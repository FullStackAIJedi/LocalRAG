namespace RagApp.Api.Requests
{
    public record AskRequest(string Question, List<Guid>? DocumentIds);
}
