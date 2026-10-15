using Microsoft.Extensions.Options;
using Verus.Application.Contracts.Relevance;
using Verus.Domain.CompanyBoundedContext.Services;

namespace Verus.Infrastructure.Relevance;

internal sealed class RelevanceEvaluator(IOptions<RelevanceOptions> options) : IRelevanceEvaluator
{
    private readonly double _minimumScore = options.Value.MinimumScore;

    private static readonly HashSet<string> NoiseTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "inc", "llc", "ltd", "limited", "corp", "corporation", "co", "company", "gmbh",
        "sa", "srl", "plc", "group", "holdings", "the", "and",
    };

    public RelevanceAssessment AssessNameWebsite(string companyName, string websiteUrl)
    {
        if (!Uri.TryCreate(websiteUrl, UriKind.Absolute, out var uri))
            return new RelevanceAssessment(0, false, "The website URL could not be parsed.");

        var label = TextSimilarity.Normalize(WebsiteDomainResolver.GetRegistrableLabel(uri));
        var tokens = Tokenize(companyName);
        var score = ScoreNameAgainstLabel(tokens, label);
        var isRelevant = score >= _minimumScore;

        var explanation = isRelevant
            ? $"Name matched domain '{label}' with a relevance score of {score:0.00}."
            : $"Relevance score {score:0.00} for domain '{label}' is below the required {_minimumScore:0.00} threshold.";

        return new RelevanceAssessment(Math.Round(score, 2), isRelevant, explanation);
    }

    public double ScoreSearch(string term, string companyName, string websiteDomain)
    {
        var normalizedTerm = TextSimilarity.Normalize(term);
        if (normalizedTerm.Length == 0)
            return 0;

        var normalizedName = TextSimilarity.Normalize(companyName);
        var normalizedDomain = TextSimilarity.Normalize(websiteDomain);

        double score = 0;

        if (normalizedName.Contains(normalizedTerm) || normalizedTerm.Contains(normalizedName))
            score = Math.Max(score, 0.9);

        if (normalizedDomain.Contains(normalizedTerm))
            score = Math.Max(score, 0.8);

        var termTokens = Tokenize(term);
        var nameTokens = Tokenize(companyName);
        if (termTokens.Count > 0)
        {
            var overlap = termTokens.Count(nameTokens.Contains) / (double)termTokens.Count;
            score = Math.Max(score, overlap);
        }

        var fuzzy = TextSimilarity.Ratio(normalizedTerm, normalizedName);
        if (fuzzy >= 0.6)
            score = Math.Max(score, fuzzy);

        return Math.Round(score, 2);
    }

    private static double ScoreNameAgainstLabel(IReadOnlyList<string> nameTokens, string label)
    {
        if (nameTokens.Count == 0 || label.Length == 0)
            return 0;

        var condensed = string.Concat(nameTokens);
        double best = 0;

        if (label.Contains(condensed) || condensed.Contains(label))
            best = Math.Max(best, 0.95);

        foreach (var token in nameTokens)
        {
            if (token.Length >= 3 && label.Contains(token))
                best = Math.Max(best, 0.80);
        }

        if (nameTokens.Count >= 2)
        {
            var acronym = string.Concat(nameTokens.Select(token => token[0]));
            if (acronym.Length >= 2 && label.Contains(acronym))
                best = Math.Max(best, 0.70);
        }

        best = Math.Max(best, TextSimilarity.Ratio(condensed, label));

        return Math.Min(best, 1.0);
    }

    private static List<string> Tokenize(string value) =>
        value
            .Split([' ', '-', '.', ',', '_', '/', '&', '\'', '"', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Select(TextSimilarity.Normalize)
            .Where(token => token.Length > 0 && !NoiseTokens.Contains(token))
            .ToList();
}
