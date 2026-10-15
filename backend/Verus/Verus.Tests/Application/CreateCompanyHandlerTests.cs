using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Verus.Application.UseCases.CompanyBoundedContext.CreateCompany;
using Verus.Domain.CompanyBoundedContext.Entities;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Tests.Application;

[TestFixture]
public class CreateCompanyHandlerTests
{
    private Mock<ICompanyRepository> _repository = null!;
    private CreateCompanyHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ICompanyRepository>();
        _handler = new CreateCompanyHandler(_repository.Object, NullLogger<CreateCompanyHandler>.Instance);
    }

    [Test]
    public async Task Handle_ValidCommand_PersistsCompany()
    {
        var command = new CreateCompanyCommand("Acme Widgets", "https://acmewidgets.com");

        await _handler.Handle(command, CancellationToken.None);

        _repository.Verify(
            r => r.AddAsync(It.Is<CompanyDomain>(c => c.Name == "Acme Widgets"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ValidCommand_ReturnsMappedResponseWithDerivedDomain()
    {
        var command = new CreateCompanyCommand("Acme Widgets", "https://www.acmewidgets.com");

        var response = await _handler.Handle(command, CancellationToken.None);

        Assert.That(response.Name, Is.EqualTo("Acme Widgets"));
        Assert.That(response.WebsiteDomain, Is.EqualTo("acmewidgets.com"));
        Assert.That(response.Id, Is.Not.EqualTo(Guid.Empty));
    }
}
