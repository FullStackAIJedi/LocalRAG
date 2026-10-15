namespace RagApp.Application.Abstractions
{
    public interface ITextChunker
    {
        List<string> Chunk(string text, int chunkSize = 1000, int overlap = 200);
    }
}
