namespace RagApp.Application.Features.Ask
{ 
    using MediatR;
    using RagApp.Application.Dtos; 
     
    public record AskQuestionQuery(
        Guid UserId,
        string Question,
        List<Guid>? DocumentIds) : IRequest<AskResponseDto>;
}
