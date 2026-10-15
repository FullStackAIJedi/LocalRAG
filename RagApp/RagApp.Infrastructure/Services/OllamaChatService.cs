namespace RagApp.Infrastructure.Services
{
    using System.Text.Json;
    using System.Net.Http.Json;

    using Microsoft.Extensions.Configuration;

    using RagApp.Application.Abstractions;

    public class OllamaChatService : IChatService
    {
        private readonly HttpClient _http;
        private readonly string _model;

        public OllamaChatService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _http.BaseAddress = new Uri(config["Ollama:BaseUrl"]!);
            _http.Timeout = TimeSpan.FromMinutes(3);
            _model = config["Ollama:ChatModel"]!;
        }

        public async Task<string> GenerateAnswerAsync(string question, string context, CancellationToken ct = default)
        {
            var systemPrompt = """
            You are an assistant that answers questions ONLY based on the provided context.
            If the answer is not in the context, say honestly: "I could not find that answer in the uploaded documents."
            Answer in the same language as the question.
            Be concise and precise.
            """;

            var payload = new
            {
                model = _model,
                messages = new[]
                {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = $"Context:\n{context}\n\nQuestion: {question}" }
            },
                stream = false
            };

            var response = await _http.PostAsJsonAsync("api/chat", payload, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            return json.GetProperty("message").GetProperty("content").GetString() ?? "";
        }
    }

}
