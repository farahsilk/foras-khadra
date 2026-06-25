using ForasKhadra.API.Models;

namespace ForasKhadra.API.Services;

/// <summary>
/// One LLM API behind a common shape. Add a new provider by implementing this
/// and registering it in Program.cs — the compare endpoint picks it up automatically.
/// </summary>
public interface ILlmProvider
{
    /// <summary>Display name, e.g. "Claude", "OpenAI", "Gemini".</summary>
    string Name { get; }

    /// <summary>True only when an API key is set (not the placeholder).</summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Sends the shared system prompt + the conversation to this provider and
    /// returns the text answer. The system prompt (with the platform data) is
    /// built once by ChatService so every provider sees identical context.
    /// </summary>
    Task<ChatResponse> AskAsync(string systemPrompt, ChatRequest request, CancellationToken ct = default);
}
