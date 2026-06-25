using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ForasKhadra.API.Models;

namespace ForasKhadra.API.Services.Providers;

/// <summary>OpenAI — Chat Completions API.</summary>
public class OpenAIProvider : ILlmProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly string _apiKey, _model, _baseUrl;
    private readonly int _maxTokens;

    public string Name => "OpenAI";
    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) && !_apiKey.StartsWith("PUT-YOUR");

    public OpenAIProvider(HttpClient http, IConfiguration config, ILogger<OpenAIProvider> logger)
    {
        _http = http;
        _logger = logger;
        var s = config.GetSection("Llm:OpenAI");
        _apiKey = s["ApiKey"] ?? "";
        _model = s["Model"] ?? "gpt-4o";
        _maxTokens = int.TryParse(s["MaxTokens"], out var m) ? m : 1024;
        _baseUrl = s["BaseUrl"] ?? "https://api.openai.com/v1/chat/completions";
    }

    public async Task<ChatResponse> AskAsync(string systemPrompt, ChatRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        if (!IsConfigured)
            return Fail("No OpenAI API key configured (Llm:OpenAI:ApiKey).", sw);

        var messages = new List<object> { new { role = "system", content = systemPrompt } };
        foreach (var t in request.History)
            messages.Add(new { role = t.Role == "assistant" ? "assistant" : "user", content = t.Content });
        messages.Add(new { role = "user", content = request.Message });

        var payload = new { model = _model, max_tokens = _maxTokens, messages };
        using var req = new HttpRequestMessage(HttpMethod.Post, _baseUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        req.Headers.Add("Authorization", $"Bearer {_apiKey}");

        try
        {
            using var resp = await _http.SendAsync(req, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI {Status}: {Body}", resp.StatusCode, raw);
                return Fail($"OpenAI returned {(int)resp.StatusCode}.", sw);
            }

            using var doc = JsonDocument.Parse(raw);
            var answer = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            sw.Stop();
            return new ChatResponse { Provider = Name, Success = true, Answer = answer.Trim(), ElapsedMs = sw.ElapsedMilliseconds };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI call failed");
            return Fail("Could not reach OpenAI.", sw);
        }
    }

    private ChatResponse Fail(string err, Stopwatch sw)
    {
        sw.Stop();
        return new ChatResponse { Provider = Name, Success = false, Error = err, ElapsedMs = sw.ElapsedMilliseconds };
    }
}
