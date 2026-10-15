namespace RagApp.Application.Features.Documents
{
    using MediatR;
    using RagApp.Application.Dtos;

    public record GetUserDocumentsQuery(Guid UserId) : IRequest<List<DocumentDto>>;
}
