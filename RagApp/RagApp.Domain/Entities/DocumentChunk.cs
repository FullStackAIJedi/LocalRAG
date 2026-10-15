namespace RagApp.Domain.Entities
{
    using Pgvector;

    // A text chunk from the document, along with its embedding vector.
    // When uploading a PDF, we split the text into chunks of approximately 1000 characters.
    // For each chunk, we generate an embedding vector using Ollama.
    // These vectors are stored in PostgreSQL using the pgvector extension.
    public class DocumentChunk
    { 
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid DocumentId { get; set; } 
        public Document Document { get; set; } = null!;

        // The sequential index of the chunk within the document.
        // For example, the first chunk is 0, the second is 1, and so on.
        // This is useful for tracking the source when generating an answer.
        public int ChunkIndex { get; set; }
         
        public string Content { get; set; } = string.Empty;

        // The embedding vector with 768 dimensions.
        // We use the Vector type from the Pgvector library.
        // The nomic-embed-text model produces 768-dimensional vectors.
        // If you change the embedding model, you must also update the dimensions in DbContext.
        public Vector Embedding { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
