namespace RagApp.Infrastructure.Services
{ 
    using RagApp.Application.Abstractions; 

    public class SimpleTextChunker : ITextChunker
    {
        public List<string> Chunk(string text, int chunkSize = 1000, int overlap = 200)
        {
            var chunks = new List<string>();

            if (string.IsNullOrWhiteSpace(text))
                return chunks;

            if (text.Length <= chunkSize)
            {
                chunks.Add(text.Trim());
                return chunks;
            }

            if (overlap >= chunkSize)
                overlap = chunkSize / 4;

            int start = 0;
            int maxIterations = (text.Length / (chunkSize - overlap)) + 10;
            int iteration = 0;

            while (start < text.Length && iteration < maxIterations)
            {
                iteration++;

                int length = Math.Min(chunkSize, text.Length - start);
                if (length <= 0) break;

                var chunk = text.Substring(start, length);

                int lastDot = chunk.LastIndexOf(". ", StringComparison.Ordinal);
                if (lastDot > chunkSize / 2)
                {
                    chunk = chunk.Substring(0, lastDot + 1);
                    length = chunk.Length;
                }

                chunks.Add(chunk.Trim());

                int advance = length - overlap;
                if (advance <= 0)
                    advance = chunkSize / 2;

                start += advance;
            }

            return chunks.Where(c => c.Length > 20).ToList();
        }
    }

}
