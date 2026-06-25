using ForasKhadra.API.Models;

namespace ForasKhadra.API.Services;

/// <summary>
/// Builds the shared system prompt (with the platform data) once, then either
/// routes the question to one provider or fans it out to all configured ones
/// for side-by-side comparison.
/// </summary>
public class ChatService
{
    private readonly DataService _data;
    private readonly IEnumerable<ILlmProvider> _providers;
    private readonly string _activeProvider;

    public ChatService(DataService data, IEnumerable<ILlmProvider> providers, IConfiguration config)
    {
        _data = data;
        _providers = providers;
        _activeProvider = config["Llm:ActiveProvider"] ?? "Claude";
    }

    private string BuildSystemPrompt() =>
        "You are the internal analytics assistant for Foras Khadra (فرص خضراء), a platform " +
        "that publishes scholarships, fellowships, internships, jobs and other opportunities and " +
        "shares them on its website and social media. You help the TEAM (not end users) understand " +
        "their data and make decisions: engagement trends, what content performs well, what followers " +
        "ask about, posting suggestions, and caption ideas. Answer ONLY from the data provided below. " +
        "If the data does not contain the answer, say so plainly. Be concise and practical. Reply in " +
        "Arabic or English to match the user's question.\n\n" +
        _data.BuildContextForPrompt();

    /// <summary>List the providers and whether each has a key — used to populate the UI.</summary>
    public IEnumerable<(string Name, bool Configured)> ListProviders() =>
        _providers.Select(p => (p.Name, p.IsConfigured));

    /// <summary>Answer with one provider (named, or the configured default).</summary>
    public Task<ChatResponse> AskAsync(ChatRequest request, string? providerName = null, CancellationToken ct = default)
    {
        var target = providerName ?? _activeProvider;
        var provider = _providers.FirstOrDefault(p => p.Name.Equals(target, StringComparison.OrdinalIgnoreCase));

        if (provider is null)
            return Task.FromResult(new ChatResponse { Success = false, Error = $"Unknown provider '{target}'." });

        return provider.AskAsync(BuildSystemPrompt(), request, ct);
    }

    /// <summary>Run the same question across every configured provider in parallel.</summary>
    public async Task<List<ChatResponse>> CompareAsync(ChatRequest request, CancellationToken ct = default)
    {
        var system = BuildSystemPrompt();
        var configured = _providers.Where(p => p.IsConfigured).ToList();

        if (configured.Count == 0)
            return new List<ChatResponse>
            {
                new() { Success = false, Error = "No providers configured. Add at least one API key under Llm:* in appsettings.json." }
            };

        var tasks = configured.Select(p => p.AskAsync(system, request, ct));
        var results = await Task.WhenAll(tasks);
        return results.OrderBy(r => r.ElapsedMs).ToList();
    }
}
