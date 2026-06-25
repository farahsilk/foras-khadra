using ForasKhadra.API.Models;
using ForasKhadra.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForasKhadra.API.Controllers;

/// <summary>
/// Chat endpoints. /message answers with one provider; /compare runs the same
/// question across every configured provider so you can judge which is best.
/// </summary>
[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chat;

    public ChatController(ChatService chat) => _chat = chat;

    /// <summary>Which providers exist and which have a key configured.</summary>
    [HttpGet("providers")]
    public IActionResult Providers() =>
        Ok(_chat.ListProviders().Select(p => new { name = p.Name, configured = p.Configured }));

    /// <summary>Answer with one provider. Optional ?provider=OpenAI overrides the default.</summary>
    [HttpPost("message")]
    public async Task<IActionResult> Message(
        [FromBody] ChatRequest request,
        [FromQuery] string? provider,
        CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new ChatResponse { Success = false, Error = "Message is empty." });

        return Ok(await _chat.AskAsync(request, provider, ct));
    }

    /// <summary>Run the same question across all configured providers in parallel.</summary>
    [HttpPost("compare")]
    public async Task<IActionResult> Compare([FromBody] ChatRequest request, CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new ChatResponse { Success = false, Error = "Message is empty." });

        return Ok(await _chat.CompareAsync(request, ct));
    }
}
