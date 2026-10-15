using Moq;
using Verus.Application.UseCases.CompanyBoundedContext.GetCompanyById;
using Verus.Domain.CompanyBoundedContext.Entities;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Tests.Application;

[TestFixture]
public class GetCompanyByIdHandlerTests
{
    private Mock<ICompanyRepository> _repository = null!;
    private GetCompanyByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ICompanyRepository>();
        _handler = new GetCompanyByIdHandler(_repository.Object);
    }

    [Test]
    public async Task Handle_CompanyExists_ReturnsResponse()
    {
        var company = CompanyDomain.CreateNew("Acme Widgets", "https://acmewidgets.com");
        _repository
            .Setup(r => r.GetByIdAsync(company.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        var response = await _handler.Handle(new GetCompanyByIdQuery(company.Id), CancellationToken.None);

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.EqualTo(company.Id));
    }

    [Test]
    public async Task Handle_CompanyDoesNotExist_ReturnsNull()
    {
        _repository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CompanyDomain?)null);

        var response = await _handler.Handle(new GetCompanyByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.That(response, Is.Null);
    }
}
