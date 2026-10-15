namespace RagApp.Application.Features.Documents
{
    using MediatR;

    using Microsoft.EntityFrameworkCore;

    using RagApp.Application.Dtos;
    using RagApp.Application.Persistence;
     
    public class GetUserDocumentsHandler : IRequestHandler<GetUserDocumentsQuery, List<DocumentDto>>
    {
        private readonly RagDbContext _db;

        public GetUserDocumentsHandler(RagDbContext db)
        {
            _db = db;
        }

        public async Task<List<DocumentDto>> Handle(GetUserDocumentsQuery request, CancellationToken ct)
        { 
            return await _db.Documents
                .Where(d => d.UserId == request.UserId)
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new DocumentDto(
                    d.Id,
                    d.FileName,
                    d.FileSize,
                    d.Status,
                    d.ChunkCount,
                    d.UploadedAt))
                .ToListAsync(ct);
        }
    }
}
