namespace RagApp.Infrastructure.Services
{
    using System.Text.Json;
    using System.Net.Http.Json;

    using Microsoft.Extensions.Configuration;

    using RagApp.Application.Abstractions;
 
    public class OllamaEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _http;
        private readonly string _model;

        public OllamaEmbeddingService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _http.BaseAddress = new Uri(config["Ollama:BaseUrl"]!);
            _http.Timeout = TimeSpan.FromSeconds(60);
            _model = config["Ollama:EmbeddingModel"]!;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
        {
            var payload = new { model = _model, input = text };
            var url = new Uri(_http.BaseAddress!, "api/embed");
            Console.WriteLine($"[Ollama Embed] POST {url} model={_model}");

            try
            {
                var response = await _http.PostAsJsonAsync("api/embed", payload, ct);
                Console.WriteLine($"[Ollama Embed] Response {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(ct);
                    Console.WriteLine($"[Ollama Embed] Error body: {errorBody}");
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
                var vector = json.GetProperty("embeddings")[0].EnumerateArray()
                    .Select(v => v.GetSingle()).ToArray();

                Console.WriteLine($"[Ollama Embed] Vector length {vector.Length}");
                return vector;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ollama Embed] Exception: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[Ollama Embed] Inner: {ex.InnerException.Message}");
                throw;
            }
        }
    }

}
