namespace RagApp.Application.Abstractions
{ 
    public interface IPdfExtractor
    {
        Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken ct = default);
    }
}
