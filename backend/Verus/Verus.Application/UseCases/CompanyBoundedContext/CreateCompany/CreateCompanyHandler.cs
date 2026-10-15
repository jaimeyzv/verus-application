using MediatR;
using Microsoft.Extensions.Logging;
using Verus.Application.UseCases.CompanyBoundedContext.Common;
using Verus.Domain.CompanyBoundedContext.Entities;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Application.UseCases.CompanyBoundedContext.CreateCompany;

internal sealed class CreateCompanyHandler(
    ICompanyRepository repository,
    ILogger<CreateCompanyHandler> logger)
    : IRequestHandler<CreateCompanyCommand, CompanyResponse>
{
    public async Task<CompanyResponse> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = CompanyDomain.CreateNew(request.Name, request.Website);
        await repository.AddAsync(company, cancellationToken);

        logger.LogInformation(
            "Created company {CompanyId} ('{CompanyName}') for domain {WebsiteDomain}.",
            company.Id,
            company.Name,
            company.WebsiteDomain);

        return company.ToResponse();
    }
}
