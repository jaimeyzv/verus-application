using Verus.Domain.CompanyBoundedContext.Entities;

namespace Verus.Tests.Domain;

[TestFixture]
public class CompanyDomainTests
{
    [Test]
    public void CreateNew_ValidInput_SetsPropertiesAndAssignsId()
    {
        var company = CompanyDomain.CreateNew("Acme Widgets", "https://www.acmewidgets.com");

        Assert.That(company.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(company.Name, Is.EqualTo("Acme Widgets"));
        Assert.That(company.Website, Is.EqualTo("https://www.acmewidgets.com/"));
        Assert.That(company.CreatedAtUtc, Is.LessThanOrEqualTo(DateTime.UtcNow));
    }

    [Test]
    public void CreateNew_StripsWwwWhenDerivingRegistrableDomain()
    {
        var company = CompanyDomain.CreateNew("Acme Widgets", "https://www.acmewidgets.com");

        Assert.That(company.WebsiteDomain, Is.EqualTo("acmewidgets.com"));
    }

    [Test]
    public void CreateNew_TrimsSurroundingWhitespaceFromName()
    {
        var company = CompanyDomain.CreateNew("  Acme Widgets  ", "https://acmewidgets.com");

        Assert.That(company.Name, Is.EqualTo("Acme Widgets"));
    }

    [Test]
    public void CreateNew_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CompanyDomain.CreateNew("  ", "https://acmewidgets.com"));
    }

    [Test]
    public void CreateNew_MalformedUrl_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CompanyDomain.CreateNew("Acme Widgets", "not-a-url"));
    }

    [Test]
    public void CreateNew_NonHttpScheme_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CompanyDomain.CreateNew("Acme Widgets", "ftp://acmewidgets.com"));
    }
}
