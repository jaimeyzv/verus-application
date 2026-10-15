using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Verus.Application.UseCases.CompanyBoundedContext.Common;

namespace Verus.Tests.WebAPI;

[TestFixture]
public class CompaniesControllerTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAll_NoCompanies_ReturnsOkWithEmptyArray()
    {
        var response = await _client.GetAsync("/api/companies");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var companies = await response.Content.ReadFromJsonAsync<List<CompanyResponse>>();
        Assert.That(companies, Is.Empty);
    }

    [Test]
    public async Task Create_ValidCompany_Returns201WithLocationHeader()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/companies",
            new { Name = "Acme Widgets", Website = "https://www.acmewidgets.com" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Location, Is.Not.Null);

        var created = await response.Content.ReadFromJsonAsync<CompanyResponse>();
        Assert.That(created!.Name, Is.EqualTo("Acme Widgets"));
        Assert.That(created.WebsiteDomain, Is.EqualTo("acmewidgets.com"));
    }

    [Test]
    public async Task Create_IrrelevantNameAndWebsite_Returns400WithProblemDetails()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/companies",
            new { Name = "Acme Widgets", Website = "https://totallyunrelateddomain.com" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var body = await response.Content.ReadAsStringAsync();
        Assert.That(body, Does.Contain("does not appear relevant"));
    }

    [Test]
    public async Task Create_MissingName_Returns400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/companies",
            new { Name = "", Website = "https://acmewidgets.com" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync($"/api/companies/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateThenGetById_ReturnsCreatedCompany()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/companies",
            new { Name = "Globex", Website = "https://globex.com" });
        var created = await createResponse.Content.ReadFromJsonAsync<CompanyResponse>();

        var getResponse = await _client.GetAsync($"/api/companies/{created!.Id}");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var fetched = await getResponse.Content.ReadFromJsonAsync<CompanyResponse>();
        Assert.That(fetched!.Id, Is.EqualTo(created.Id));
    }

    [Test]
    public async Task Search_ByRelevanceTerm_ReturnsMatchesOrderedByRelevance()
    {
        await _client.PostAsJsonAsync("/api/companies", new { Name = "Acme Widgets", Website = "https://acmewidgets.com" });
        await _client.PostAsJsonAsync("/api/companies", new { Name = "Globex", Website = "https://globex.com" });

        var response = await _client.GetAsync("/api/companies/search?term=globex");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var results = await response.Content.ReadFromJsonAsync<List<CompanyResponse>>();
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results![0].Name, Is.EqualTo("Globex"));
    }
}
