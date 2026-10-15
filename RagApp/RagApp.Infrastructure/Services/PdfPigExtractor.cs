namespace RagApp.Infrastructure.Services
{
    using UglyToad.PdfPig;

    using RagApp.Application.Abstractions;
     
    public class PdfPigExtractor : IPdfExtractor
    { 
        public Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken ct = default)
        {
            return Task.Run(() =>
            { 
                var sb = new System.Text.StringBuilder();
                 
                using var document = PdfDocument.Open(pdfStream);
                 
                foreach (var page in document.GetPages())
                {
                    var text = page.Text;
                     
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sb.AppendLine(text);
                    }
                }

                return sb.ToString();
            }, ct);
        }
    }
}
