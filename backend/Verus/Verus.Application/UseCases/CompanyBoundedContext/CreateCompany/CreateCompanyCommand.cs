using MediatR;
using Verus.Application.UseCases.CompanyBoundedContext.Common;

namespace Verus.Application.UseCases.CompanyBoundedContext.CreateCompany;

public sealed record CreateCompanyCommand(string Name, string Website) : IRequest<CompanyResponse>;
