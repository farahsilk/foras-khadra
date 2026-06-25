using System.Text.Json.Serialization;

namespace ForasKhadra.API.Models;

/// <summary>
/// Top-level shape of foras_khadra_data.json. DataService deserializes into this.
/// </summary>
public class ForasKhadraData
{
    [JsonPropertyName("opportunities")]
    public List<Opportunity> Opportunities { get; set; } = new();

    [JsonPropertyName("social_posts")]
    public List<SocialPost> SocialPosts { get; set; } = new();

    [JsonPropertyName("analytics")]
    public Analytics Analytics { get; set; } = new();
}

public class Opportunity
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("countries")]
    public string Countries { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";

    [JsonPropertyName("posted_date")]
    public string PostedDate { get; set; } = "";

    [JsonPropertyName("deadline")]
    public string Deadline { get; set; } = "";

    [JsonPropertyName("views")]
    public int Views { get; set; }
}

public class SocialPost
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = "";

    [JsonPropertyName("opportunity_type")]
    public string OpportunityType { get; set; } = "";

    [JsonPropertyName("caption")]
    public string Caption { get; set; } = "";

    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("likes")]
    public int Likes { get; set; }

    [JsonPropertyName("comments")]
    public int Comments { get; set; }

    [JsonPropertyName("shares")]
    public int Shares { get; set; }

    [JsonPropertyName("saves")]
    public int Saves { get; set; }

    [JsonPropertyName("reach")]
    public int Reach { get; set; }

    [JsonPropertyName("engagement_rate")]
    public double EngagementRate { get; set; }

    [JsonPropertyName("sample_comments")]
    public List<string> SampleComments { get; set; } = new();
}

public class Analytics
{
    [JsonPropertyName("monthly_visitors")]
    public List<MonthlyVisitors> MonthlyVisitors { get; set; } = new();

    [JsonPropertyName("opportunities_by_type")]
    public Dictionary<string, int> OpportunitiesByType { get; set; } = new();

    [JsonPropertyName("opportunities_by_country")]
    public Dictionary<string, int> OpportunitiesByCountry { get; set; } = new();

    [JsonPropertyName("platform_summary")]
    public List<PlatformSummary> PlatformSummary { get; set; } = new();
}

public class MonthlyVisitors
{
    [JsonPropertyName("month")]
    public string Month { get; set; } = "";

    [JsonPropertyName("visitors")]
    public int Visitors { get; set; }
}

public class PlatformSummary
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = "";

    [JsonPropertyName("followers")]
    public int Followers { get; set; }

    [JsonPropertyName("total_posts")]
    public int TotalPosts { get; set; }

    [JsonPropertyName("avg_engagement_rate")]
    public double AvgEngagementRate { get; set; }

    [JsonPropertyName("total_reach")]
    public int TotalReach { get; set; }
}
