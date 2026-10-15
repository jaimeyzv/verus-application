using Verus.Domain.CompanyBoundedContext.Entities;

namespace Verus.Application.UseCases.CompanyBoundedContext.Common;

internal static class CompanyMappingExtensions
{
    public static CompanyResponse ToResponse(this CompanyDomain company) => new(
        company.Id,
        company.Name,
        company.Website,
        company.WebsiteDomain,
        company.CreatedAtUtc);
}
