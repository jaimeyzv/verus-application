using MediatR;
using Verus.Application.UseCases.CompanyBoundedContext.Common;

namespace Verus.Application.UseCases.CompanyBoundedContext.GetAllCompanies;

public sealed record GetAllCompaniesQuery : IRequest<IReadOnlyList<CompanyResponse>>;
