namespace RagApp.Domain.Entities
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = string.Empty;

        // Pending, Processing, Completed, Failed 
        public string Status { get; set; } = "Pending";

        // The number of text chunks extracted from this document.
        // Populated after successful processing.
        public int ChunkCount { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
         
        public ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
    }
}
