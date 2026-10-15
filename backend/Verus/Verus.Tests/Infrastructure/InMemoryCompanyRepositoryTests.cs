using Verus.Domain.CompanyBoundedContext.Entities;
using Verus.Infrastructure.Persistence.InMemory;

namespace Verus.Tests.Infrastructure;

[TestFixture]
public class InMemoryCompanyRepositoryTests
{
    private InMemoryCompanyRepository _repository = null!;

    [SetUp]
    public void SetUp() => _repository = new InMemoryCompanyRepository();

    [Test]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsSameCompany()
    {
        var company = CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com");

        await _repository.AddAsync(company);
        var fetched = await _repository.GetByIdAsync(company.Id);

        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched!.Id, Is.EqualTo(company.Id));
    }

    [Test]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var fetched = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.That(fetched, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_NoCompanies_ReturnsEmptyList()
    {
        var all = await _repository.GetAllAsync();

        Assert.That(all, Is.Empty);
    }

    [Test]
    public async Task SearchAsync_FiltersByPartialNameCaseInsensitively()
    {
        await _repository.AddAsync(CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com"));
        await _repository.AddAsync(CompanyDomain.CreateNew("Globex", "https://globex.com"));

        var result = await _repository.SearchAsync(name: "acme", domain: null);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Acme Widgets"));
    }

    [Test]
    public async Task SearchAsync_FiltersByWebsiteDomain()
    {
        await _repository.AddAsync(CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com"));
        await _repository.AddAsync(CompanyDomain.CreateNew("Globex", "https://globex.com"));

        var result = await _repository.SearchAsync(name: null, domain: "globex.com");

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Globex"));
    }
}
