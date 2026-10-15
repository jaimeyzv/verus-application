namespace Verus.Infrastructure.Relevance;

public sealed class RelevanceOptions
{
    public const string SectionName = "Relevance";

    public double MinimumScore { get; set; } = 0.35;
}
