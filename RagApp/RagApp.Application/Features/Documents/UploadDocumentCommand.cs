namespace RagApp.Application.Features.Documents
{
    using MediatR;
     
    public record UploadDocumentCommand(
        Guid UserId,
        string FileName,
        Stream FileStream,
        long FileSize) : IRequest<Guid>;
}
