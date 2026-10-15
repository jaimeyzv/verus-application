using Microsoft.Extensions.Options;
using Moq;
using Verus.Application.UseCases.CompanyBoundedContext.SearchCompanies;
using Verus.Domain.CompanyBoundedContext.Entities;
using Verus.Domain.CompanyBoundedContext.Repositories;
using Verus.Infrastructure.Relevance;

namespace Verus.Tests.Application;

[TestFixture]
public class SearchCompaniesHandlerTests
{
    private Mock<ICompanyRepository> _repository = null!;
    private SearchCompaniesHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ICompanyRepository>();
        var evaluator = new RelevanceEvaluator(Options.Create(new RelevanceOptions()));
        _handler = new SearchCompaniesHandler(_repository.Object, evaluator);
    }

    [Test]
    public async Task Handle_WithTerm_OrdersResultsByRelevanceDescending()
    {
        var acme = CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com");
        var globex = CompanyDomain.CreateNew("Globex", "https://globex.com");
        _repository
            .Setup(r => r.SearchAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync([acme, globex]);

        var result = await _handler.Handle(new SearchCompaniesQuery(null, null, "globex"), CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Globex"));
    }

    [Test]
    public async Task Handle_WithoutTerm_ReturnsFilteredResultsOrderedByName()
    {
        var globex = CompanyDomain.CreateNew("Globex", "https://globex.com");
        var acme = CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com");
        _repository
            .Setup(r => r.SearchAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync([globex, acme]);

        var result = await _handler.Handle(new SearchCompaniesQuery(null, null, null), CancellationToken.None);

        Assert.That(result.Select(c => c.Name), Is.EqualTo(new[] { "Acme Widgets", "Globex" }));
    }

    [Test]
    public async Task Handle_WithTermMatchingNothing_ReturnsEmptyList()
    {
        var acme = CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com");
        _repository
            .Setup(r => r.SearchAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync([acme]);

        var result = await _handler.Handle(new SearchCompaniesQuery(null, null, "zzzzz"), CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}
