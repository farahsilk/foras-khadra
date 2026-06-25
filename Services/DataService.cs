using System.Text;
using System.Text.Json;
using ForasKhadra.API.Models;

namespace ForasKhadra.API.Services;

/// <summary>
/// Loads foras_khadra_data.json once at startup and keeps it in memory.
/// Everything else (charts + chatbot) reads from here. No database needed.
/// </summary>
public class DataService
{
    private readonly ForasKhadraData _data;
    private readonly ILogger<DataService> _logger;

    public DataService(IWebHostEnvironment env, ILogger<DataService> logger)
    {
        _logger = logger;
        var path = Path.Combine(env.ContentRootPath, "Data", "foras_khadra_data.json");

        if (!File.Exists(path))
        {
            _logger.LogError("Data file not found at {Path}", path);
            _data = new ForasKhadraData();
            return;
        }

        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _data = JsonSerializer.Deserialize<ForasKhadraData>(json, options) ?? new ForasKhadraData();

        _logger.LogInformation(
            "Loaded {Opps} opportunities, {Posts} social posts.",
            _data.Opportunities.Count, _data.SocialPosts.Count);
    }

    public ForasKhadraData Raw => _data;

    private static bool IsOpen(string status) =>
        status.Contains("متاح") || status.Equals("open", StringComparison.OrdinalIgnoreCase);

    // ---------- Analytics used by the dashboard ----------

    public OverviewDto GetOverview()
    {
        var open = _data.Opportunities.Count(o => IsOpen(o.Status));
        var posts = _data.SocialPosts;

        return new OverviewDto
        {
            TotalOpportunities = _data.Opportunities.Count,
            OpenOpportunities = open,
            ClosedOpportunities = _data.Opportunities.Count - open,
            LatestMonthVisitors = _data.Analytics.MonthlyVisitors.LastOrDefault()?.Visitors ?? 0,
            TotalSocialPosts = posts.Count,
            TotalReach = posts.Sum(p => (long)p.Reach),
            AvgEngagementRate = posts.Count > 0
                ? Math.Round(posts.Average(p => p.EngagementRate), 2)
                : 0
        };
    }

    public List<LabelValueDto> GetMonthlyVisitors() =>
        _data.Analytics.MonthlyVisitors
            .Select(m => new LabelValueDto { Label = m.Month, Value = m.Visitors })
            .ToList();

    public List<LabelValueDto> GetByType()
    {
        // Prefer the precomputed analytics; fall back to counting opportunities.
        if (_data.Analytics.OpportunitiesByType.Count > 0)
        {
            return _data.Analytics.OpportunitiesByType
                .Select(kv => new LabelValueDto { Label = kv.Key, Value = kv.Value })
                .OrderByDescending(x => x.Value)
                .ToList();
        }

        return _data.Opportunities
            .GroupBy(o => o.Type)
            .Select(g => new LabelValueDto { Label = g.Key, Value = g.Count() })
            .OrderByDescending(x => x.Value)
            .ToList();
    }

    public List<LabelValueDto> GetEngagementByPlatform() =>
        _data.SocialPosts
            .GroupBy(p => p.Platform)
            .Select(g => new LabelValueDto
            {
                Label = g.Key,
                Value = Math.Round(g.Average(p => p.EngagementRate), 2)
            })
            .OrderByDescending(x => x.Value)
            .ToList();

    public List<TopOpportunityDto> GetTopOpportunities(int take = 8) =>
        _data.Opportunities
            .OrderByDescending(o => o.Views)
            .Take(take)
            .Select(o => new TopOpportunityDto
            {
                Title = o.Title,
                Type = o.Type,
                Status = IsOpen(o.Status) ? "متاح" : "منتهي",
                Views = o.Views
            })
            .ToList();

    // ---------- Context builder for the chatbot ----------

    /// <summary>
    /// Builds a compact, token-friendly summary of the data to inject into the
    /// Claude prompt. We summarize rather than dump the whole JSON so the prompt
    /// stays small and cheap while still being grounded in the real numbers.
    /// </summary>
    public string BuildContextForPrompt()
    {
        var o = GetOverview();
        var sb = new StringBuilder();

        sb.AppendLine("=== Foras Khadra — platform data summary ===");
        sb.AppendLine($"Total opportunities: {o.TotalOpportunities} (open: {o.OpenOpportunities}, closed: {o.ClosedOpportunities})");
        sb.AppendLine($"Latest month visitors: {o.LatestMonthVisitors:N0}");
        sb.AppendLine($"Social posts: {o.TotalSocialPosts}, total reach: {o.TotalReach:N0}, avg engagement rate: {o.AvgEngagementRate}%");
        sb.AppendLine();

        sb.AppendLine("Opportunities by type:");
        foreach (var t in GetByType())
            sb.AppendLine($"  - {t.Label}: {t.Value}");
        sb.AppendLine();

        sb.AppendLine("Avg engagement rate by platform:");
        foreach (var p in GetEngagementByPlatform())
            sb.AppendLine($"  - {p.Label}: {p.Value}%");
        sb.AppendLine();

        sb.AppendLine("Platform follower summary:");
        foreach (var p in _data.Analytics.PlatformSummary)
            sb.AppendLine($"  - {p.Platform}: {p.Followers:N0} followers, {p.TotalPosts} posts, {p.AvgEngagementRate}% avg engagement, reach {p.TotalReach:N0}");
        sb.AppendLine();

        sb.AppendLine("Monthly visitors trend:");
        foreach (var m in _data.Analytics.MonthlyVisitors)
            sb.AppendLine($"  - {m.Month}: {m.Visitors:N0}");
        sb.AppendLine();

        // Top engaging posts give the model concrete signal about what performs well.
        sb.AppendLine("Top 10 social posts by engagement rate:");
        foreach (var p in _data.SocialPosts.OrderByDescending(x => x.EngagementRate).Take(10))
        {
            sb.AppendLine($"  - [{p.Platform}] type={p.OpportunityType} eng={p.EngagementRate}% likes={p.Likes} comments={p.Comments} shares={p.Shares} | \"{Trim(p.Caption, 80)}\"");
        }
        sb.AppendLine();

        // A sample of what followers ask about, so the assistant can reason about audience needs.
        var comments = _data.SocialPosts
            .SelectMany(p => p.SampleComments)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Take(20)
            .ToList();
        if (comments.Count > 0)
        {
            sb.AppendLine("Sample follower comments:");
            foreach (var c in comments)
                sb.AppendLine($"  - {Trim(c, 100)}");
        }

        return sb.ToString();
    }

    private static string Trim(string s, int max) =>
        string.IsNullOrEmpty(s) || s.Length <= max ? s : s[..max] + "…";
}
