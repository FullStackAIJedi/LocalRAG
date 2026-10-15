namespace RagApp.Application.Dtos
{
    public record SourceChunkDto(
        Guid DocumentId,
        string FileName,
        int ChunkIndex,
        string Preview,
        double Score);
}
