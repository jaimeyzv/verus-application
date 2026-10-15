namespace Verus.Application.UseCases.CompanyBoundedContext.Common;

public sealed record CompanyResponse(
    Guid Id,
    string Name,
    string Website,
    string WebsiteDomain,
    DateTime CreatedAtUtc);
