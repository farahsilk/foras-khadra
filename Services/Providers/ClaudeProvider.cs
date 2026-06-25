using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ForasKhadra.API.Models;

namespace ForasKhadra.API.Services.Providers;

/// <summary>Anthropic Claude — Messages API.</summary>
public class ClaudeProvider : ILlmProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<ClaudeProvider> _logger;
    private readonly string _apiKey, _model, _baseUrl, _version;
    private readonly int _maxTokens;

    public string Name => "Claude";
    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) && !_apiKey.StartsWith("PUT-YOUR");

    public ClaudeProvider(HttpClient http, IConfiguration config, ILogger<ClaudeProvider> logger)
    {
        _http = http;
        _logger = logger;
        var s = config.GetSection("Llm:Claude");
        _apiKey = s["ApiKey"] ?? "";
        _model = s["Model"] ?? "claude-sonnet-4-6";
        _maxTokens = int.TryParse(s["MaxTokens"], out var m) ? m : 1024;
        _baseUrl = s["BaseUrl"] ?? "https://api.anthropic.com/v1/messages";
        _version = s["AnthropicVersion"] ?? "2023-06-01";
    }

    public async Task<ChatResponse> AskAsync(string systemPrompt, ChatRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        if (!IsConfigured)
            return Fail("No Claude API key configured (Llm:Claude:ApiKey).", sw);

        var messages = new List<object>();
        foreach (var t in request.History)
            messages.Add(new { role = t.Role == "assistant" ? "assistant" : "user", content = t.Content });
        messages.Add(new { role = "user", content = request.Message });

        var payload = new { model = _model, max_tokens = _maxTokens, system = systemPrompt, messages };
        using var req = new HttpRequestMessage(HttpMethod.Post, _baseUrl)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        req.Headers.Add("x-api-key", _apiKey);
        req.Headers.Add("anthropic-version", _version);

        try
        {
            using var resp = await _http.SendAsync(req, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Claude {Status}: {Body}", resp.StatusCode, raw);
                return Fail($"Claude returned {(int)resp.StatusCode}.", sw);
            }

            using var doc = JsonDocument.Parse(raw);
            var sb = new StringBuilder();
            foreach (var block in doc.RootElement.GetProperty("content").EnumerateArray())
                if (block.GetProperty("type").GetString() == "text")
                    sb.Append(block.GetProperty("text").GetString());

            sw.Stop();
            return new ChatResponse { Provider = Name, Success = true, Answer = sb.ToString().Trim(), ElapsedMs = sw.ElapsedMilliseconds };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Claude call failed");
            return Fail("Could not reach Claude.", sw);
        }
    }

    private ChatResponse Fail(string err, Stopwatch sw)
    {
        sw.Stop();
        return new ChatResponse { Provider = Name, Success = false, Error = err, ElapsedMs = sw.ElapsedMilliseconds };
    }
}
