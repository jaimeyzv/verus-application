using MediatR;
using Verus.Application.UseCases.CompanyBoundedContext.Common;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Application.UseCases.CompanyBoundedContext.GetAllCompanies;

internal sealed class GetAllCompaniesHandler(ICompanyRepository repository)
    : IRequestHandler<GetAllCompaniesQuery, IReadOnlyList<CompanyResponse>>
{
    public async Task<IReadOnlyList<CompanyResponse>> Handle(
        GetAllCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await repository.GetAllAsync(cancellationToken);
        return companies
            .OrderBy(company => company.Name, StringComparer.OrdinalIgnoreCase)
            .Select(company => company.ToResponse())
            .ToList();
    }
}
