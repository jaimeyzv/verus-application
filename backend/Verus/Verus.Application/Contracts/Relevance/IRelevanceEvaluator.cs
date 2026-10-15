namespace Verus.Application.Contracts.Relevance;

public interface IRelevanceEvaluator
{
    RelevanceAssessment AssessNameWebsite(string companyName, string websiteUrl);

    double ScoreSearch(string term, string companyName, string websiteDomain);
}
