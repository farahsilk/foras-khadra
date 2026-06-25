using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ForasKhadra.API.Models;

namespace ForasKhadra.API.Services.Providers;

/// <summary>Google Gemini — generateContent API.</summary>
public class GeminiProvider : ILlmProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<GeminiProvider> _logger;
    private readonly string _apiKey, _model, _baseUrl;
    private readonly int _maxTokens;

    public string Name => "Gemini";
    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) && !_apiKey.StartsWith("PUT-YOUR");

    public GeminiProvider(HttpClient http, IConfiguration config, ILogger<GeminiProvider> logger)
    {
        _http = http;
        _logger = logger;
        var s = config.GetSection("Llm:Gemini");
        _apiKey = s["ApiKey"] ?? "";
        _model = s["Model"] ?? "gemini-1.5-flash";
        _maxTokens = int.TryParse(s["MaxTokens"], out var m) ? m : 1024;
        _baseUrl = s["BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models";
    }

    public async Task<ChatResponse> AskAsync(string systemPrompt, ChatRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        if (!IsConfigured)
            return Fail("No Gemini API key configured (Llm:Gemini:ApiKey).", sw);

        // Gemini roles are "user" and "model"; the system prompt goes in system_instruction.
        var contents = new List<object>();
        foreach (var t in request.History)
            contents.Add(new { role = t.Role == "assistant" ? "model" : "user", parts = new[] { new { text = t.Content } } });
        contents.Add(new { role = "user", parts = new[] { new { text = request.Message } } });

        var payload = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents,
            generationConfig = new { maxOutputTokens = _maxTokens }
        };

        var url = $"{_baseUrl}/{_model}:generateContent?key={_apiKey}";
        using var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        try
        {
            using var resp = await _http.SendAsync(req, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini {Status}: {Body}", resp.StatusCode, raw);
                return Fail($"Gemini returned {(int)resp.StatusCode}.", sw);
            }

            using var doc = JsonDocument.Parse(raw);
            var sb = new StringBuilder();
            foreach (var part in doc.RootElement
                         .GetProperty("candidates")[0]
                         .GetProperty("content")
                         .GetProperty("parts").EnumerateArray())
            {
                if (part.TryGetProperty("text", out var text))
                    sb.Append(text.GetString());
            }

            sw.Stop();
            return new ChatResponse { Provider = Name, Success = true, Answer = sb.ToString().Trim(), ElapsedMs = sw.ElapsedMilliseconds };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini call failed");
            return Fail("Could not reach Gemini.", sw);
        }
    }

    private ChatResponse Fail(string err, Stopwatch sw)
    {
        sw.Stop();
        return new ChatResponse { Provider = Name, Success = false, Error = err, ElapsedMs = sw.ElapsedMilliseconds };
    }
}
