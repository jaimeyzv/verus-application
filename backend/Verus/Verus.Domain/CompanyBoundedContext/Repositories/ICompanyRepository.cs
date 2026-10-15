using Verus.Domain.CompanyBoundedContext.Entities;

namespace Verus.Domain.CompanyBoundedContext.Repositories;

public interface ICompanyRepository
{
    Task<CompanyDomain> AddAsync(CompanyDomain company, CancellationToken cancellationToken = default);

    Task<CompanyDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompanyDomain>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompanyDomain>> SearchAsync(string? name, string? domain, CancellationToken cancellationToken = default);
}
