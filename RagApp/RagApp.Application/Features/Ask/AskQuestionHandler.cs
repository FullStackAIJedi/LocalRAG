namespace RagApp.Application.Features.Ask
{
    using Microsoft.EntityFrameworkCore;

    using MediatR;

    using Pgvector;

    using Pgvector.EntityFrameworkCore;
    using RagApp.Application.Abstractions;
    using RagApp.Application.Dtos;
    using RagApp.Application.Persistence;
     
    public class AskQuestionHandler : IRequestHandler<AskQuestionQuery, AskResponseDto>
    {
        private readonly RagDbContext _db;
        private readonly IEmbeddingService _embedding;
        private readonly IChatService _chat; 
        private const int TopK = 5;

        public AskQuestionHandler(RagDbContext db, IEmbeddingService embedding, IChatService chat)
        {
            _db = db;
            _embedding = embedding;
            _chat = chat;
        }

        public async Task<AskResponseDto> Handle(AskQuestionQuery request, CancellationToken ct)
        { 
            var queryEmbedding = new Vector(await _embedding.GenerateEmbeddingAsync(request.Question, ct));
             
            var query = _db.Chunks
                .Include(c => c.Document)
                .Where(c => c.Document.UserId == request.UserId);
             
            if (request.DocumentIds != null && request.DocumentIds.Count > 0)
            {
                query = query.Where(c => request.DocumentIds.Contains(c.DocumentId));
            }
             
            var topChunks = await query
                .OrderBy(c => c.Embedding.CosineDistance(queryEmbedding))
                .Take(TopK)
                .Select(c => new
                {
                    c.Id,
                    c.DocumentId,
                    FileName = c.Document.FileName,
                    c.ChunkIndex,
                    c.Content,
                    Score = c.Embedding.CosineDistance(queryEmbedding)
                })
                .ToListAsync(ct);
             
            if (topChunks.Count == 0)
            {
                return new AskResponseDto(
                    "Не намирам качени документи, в които да търся.",
                    new List<SourceChunkDto>());
            }
             
            var context = string.Join("\n\n---\n\n",
                topChunks.Select((c, i) => $"[Източник {i + 1}: {c.FileName}, блок {c.ChunkIndex}]\n{c.Content}"));
             
            var answer = await _chat.GenerateAnswerAsync(request.Question, context, ct);
             
            var sources = topChunks.Select(c => new SourceChunkDto(
                c.DocumentId,
                c.FileName,
                c.ChunkIndex, 
                c.Content.Length > 200 ? c.Content.Substring(0, 200) + "..." : c.Content,
                1 - c.Score
            )).ToList();

            return new AskResponseDto(answer, sources);
        }
    }
}
