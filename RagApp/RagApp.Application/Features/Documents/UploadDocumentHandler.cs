namespace RagApp.Application.Features.Documents
{
    using Microsoft.Extensions.Configuration;

    using MediatR;
    using Pgvector;

    using RagApp.Application.Abstractions;
    using RagApp.Application.Persistence;
    using RagApp.Domain.Entities;

    public class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, Guid>
    {
        private readonly RagDbContext _db;
        private readonly IPdfExtractor _pdf;
        private readonly ITextChunker _chunker;
        private readonly IEmbeddingService _embedding;
        private readonly IConfiguration _config;

        public UploadDocumentHandler(
            RagDbContext db,
            IPdfExtractor pdf,
            ITextChunker chunker,
            IEmbeddingService embedding,
            IConfiguration config)
        {
            _db = db;
            _pdf = pdf;
            _chunker = chunker;
            _embedding = embedding;
            _config = config;
        }

        public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken ct)
        {
            Console.WriteLine("=== UPLOAD HANDLER v3 STARTED ===");
            Console.WriteLine($"[Upload] File: {request.FileName}, Size: {request.FileSize}");

            var root = _config["FileStorage:RootPath"]!;
            Directory.CreateDirectory(root);

            var docId = Guid.NewGuid();
            var storedPath = Path.Combine(root, $"{docId}_{request.FileName}");

            using (var fs = File.Create(storedPath))
            {
                request.FileStream.Position = 0;
                await request.FileStream.CopyToAsync(fs, ct);
            }
            Console.WriteLine($"[Upload] File saved: {storedPath}");

            var document = new Document
            {
                Id = docId,
                UserId = request.UserId,
                FileName = request.FileName,
                FileSize = request.FileSize,
                StoragePath = storedPath,
                Status = "Processing"
            };
            _db.Documents.Add(document);
            await _db.SaveChangesAsync(ct);
            Console.WriteLine($"[Upload] Document record created: {docId}");

            try
            {
                request.FileStream.Position = 0;
                Console.WriteLine("[Upload] Extracting text from PDF...");
                var text = await _pdf.ExtractTextAsync(request.FileStream, ct);
                Console.WriteLine($"[Upload] Extracted {text.Length} characters");

                if (string.IsNullOrWhiteSpace(text))
                {
                    document.Status = "Failed";
                    await _db.SaveChangesAsync(ct);
                    throw new InvalidOperationException("PDF does not contain extractable text.");
                }

                var chunks = _chunker.Chunk(text);
                Console.WriteLine($"[Upload] Chunked into {chunks.Count} blocks");

                for (int i = 0; i < chunks.Count; i++)
                {
                    Console.WriteLine($"[Upload] Embedding block {i + 1}/{chunks.Count}, length {chunks[i].Length}");

                    var embeddingArray = await _embedding.GenerateEmbeddingAsync(chunks[i], ct);
                    Console.WriteLine($"[Upload] Got embedding with {embeddingArray.Length} dimensions");

                    _db.Chunks.Add(new DocumentChunk
                    {
                        DocumentId = docId,
                        ChunkIndex = i,
                        Content = chunks[i],
                        Embedding = new Vector(embeddingArray)
                    });

                    if (i % 20 == 0)
                    {
                        await _db.SaveChangesAsync(ct);
                        Console.WriteLine($"[Upload] Batch saved at block {i + 1}");
                    }
                }

                document.ChunkCount = chunks.Count;
                document.Status = "Completed";
                await _db.SaveChangesAsync(ct);
                Console.WriteLine($"[Upload] COMPLETED with {chunks.Count} blocks");

                return docId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Upload] FAILED: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[Upload] Inner: {ex.InnerException.Message}");

                document.Status = "Failed";
                await _db.SaveChangesAsync(ct);
                throw;
            }
        }
    }
}
