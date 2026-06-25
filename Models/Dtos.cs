namespace ForasKhadra.API.Models;

/// <summary>What the chat page sends to the server.</summary>
public class ChatRequest
{
    public string Message { get; set; } = "";

    /// <summary>Optional prior turns so the assistant keeps context.</summary>
    public List<ChatTurn> History { get; set; } = new();
}

public class ChatTurn
{
    public string Role { get; set; } = "user"; // "user" or "assistant"
    public string Content { get; set; } = "";
}

public class ChatResponse
{
    public string Answer { get; set; } = "";
    public bool Success { get; set; } = true;
    public string? Error { get; set; }

    /// <summary>Which model produced this answer (e.g. "Claude", "OpenAI", "Gemini").</summary>
    public string Provider { get; set; } = "";

    /// <summary>Round-trip time in milliseconds — useful when comparing models.</summary>
    public long ElapsedMs { get; set; }
}

/// <summary>Headline numbers for the dashboard cards.</summary>
public class OverviewDto
{
    public int TotalOpportunities { get; set; }
    public int OpenOpportunities { get; set; }
    public int ClosedOpportunities { get; set; }
    public int LatestMonthVisitors { get; set; }
    public int TotalSocialPosts { get; set; }
    public long TotalReach { get; set; }
    public double AvgEngagementRate { get; set; }
}

public class LabelValueDto
{
    public string Label { get; set; } = "";
    public double Value { get; set; }
}

public class TopOpportunityDto
{
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public int Views { get; set; }
}
