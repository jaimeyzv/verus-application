using MediatR;
using Verus.Application.UseCases.CompanyBoundedContext.Common;

namespace Verus.Application.UseCases.CompanyBoundedContext.SearchCompanies;

public sealed record SearchCompaniesQuery(string? Name, string? Domain, string? Term)
    : IRequest<IReadOnlyList<CompanyResponse>>;
