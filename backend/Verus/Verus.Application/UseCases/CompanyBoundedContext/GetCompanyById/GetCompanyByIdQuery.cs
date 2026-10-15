using MediatR;
using Verus.Application.UseCases.CompanyBoundedContext.Common;

namespace Verus.Application.UseCases.CompanyBoundedContext.GetCompanyById;

public sealed record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyResponse?>;
