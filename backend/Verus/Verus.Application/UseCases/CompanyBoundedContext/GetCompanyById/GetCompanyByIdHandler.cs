using MediatR;
using Verus.Application.UseCases.CompanyBoundedContext.Common;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Application.UseCases.CompanyBoundedContext.GetCompanyById;

internal sealed class GetCompanyByIdHandler(ICompanyRepository repository)
    : IRequestHandler<GetCompanyByIdQuery, CompanyResponse?>
{
    public async Task<CompanyResponse?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await repository.GetByIdAsync(request.Id, cancellationToken);
        return company?.ToResponse();
    }
}
