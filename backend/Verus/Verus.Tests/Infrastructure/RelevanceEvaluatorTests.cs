using Microsoft.Extensions.Options;
using Verus.Infrastructure.Relevance;

namespace Verus.Tests.Infrastructure;

[TestFixture]
public class RelevanceEvaluatorTests
{
    private static RelevanceEvaluator CreateEvaluator(double minimumScore = 0.35) =>
        new(Options.Create(new RelevanceOptions { MinimumScore = minimumScore }));

    [Test]
    public void AssessNameWebsite_NameMatchesDomain_IsRelevant()
    {
        var evaluator = CreateEvaluator();

        var result = evaluator.AssessNameWebsite("Acme Widgets", "https://www.acmewidgets.com");

        Assert.That(result.IsRelevant, Is.True);
        Assert.That(result.Score, Is.GreaterThanOrEqualTo(0.35));
    }

    [Test]
    public void AssessNameWebsite_StripsLegalSuffixBeforeMatching()
    {
        var evaluator = CreateEvaluator();

        var result = evaluator.AssessNameWebsite("Example Inc", "https://example.com");

        Assert.That(result.IsRelevant, Is.True);
    }

    [Test]
    public void AssessNameWebsite_AcronymFormsDomain_IsRelevant()
    {
        var evaluator = CreateEvaluator();

        var result = evaluator.AssessNameWebsite("International Business Machines", "https://ibm.com");

        Assert.That(result.IsRelevant, Is.True);
    }

    [Test]
    public void AssessNameWebsite_UnrelatedNameAndDomain_IsNotRelevant()
    {
        var evaluator = CreateEvaluator();

        var result = evaluator.AssessNameWebsite("Acme Widgets", "https://totallyunrelateddomain.com");

        Assert.That(result.IsRelevant, Is.False);
        Assert.That(result.Explanation, Does.Contain("threshold"));
    }

    [Test]
    public void ScoreSearch_TermAppearsInName_ReturnsHighScore()
    {
        var evaluator = CreateEvaluator();

        var score = evaluator.ScoreSearch("acme", "Acme Widgets", "acmewidgets.com");

        Assert.That(score, Is.GreaterThanOrEqualTo(0.8));
    }

    [Test]
    public void ScoreSearch_NoMatch_ReturnsZero()
    {
        var evaluator = CreateEvaluator();

        var score = evaluator.ScoreSearch("zzzzz", "Acme Widgets", "acmewidgets.com");

        Assert.That(score, Is.EqualTo(0));
    }
}
