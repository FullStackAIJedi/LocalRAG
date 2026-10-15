namespace RagApp.Application.Dtos
{
    public record AskResponseDto(
        string Answer,
        List<SourceChunkDto> Sources);
}
