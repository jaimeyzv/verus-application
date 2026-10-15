namespace Verus.Application.Contracts.Relevance;

public sealed record RelevanceAssessment(double Score, bool IsRelevant, string Explanation);
