namespace RagApp.Application.Dtos
{
    public record DocumentDto(
        Guid Id,
        string FileName,
        long FileSize,
        string Status,
        int ChunkCount,
        DateTime UploadedAt);
}
