using System.Collections.Concurrent;
using Verus.Domain.CompanyBoundedContext.Entities;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Infrastructure.Persistence.InMemory;

internal sealed class InMemoryCompanyRepository : ICompanyRepository
{
    private readonly ConcurrentDictionary<Guid, CompanyDomain> _companies = new();

    public Task<CompanyDomain> AddAsync(CompanyDomain company, CancellationToken cancellationToken = default)
    {
        _companies[company.Id] = company;
        return Task.FromResult(company);
    }

    public Task<CompanyDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _companies.TryGetValue(id, out var company);
        return Task.FromResult(company);
    }

    public Task<IReadOnlyList<CompanyDomain>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CompanyDomain> snapshot = _companies.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<IReadOnlyList<CompanyDomain>> SearchAsync(
        string? name,
        string? domain,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<CompanyDomain> query = _companies.Values;

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(company => company.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(domain))
            query = query.Where(company => company.WebsiteDomain.Contains(domain, StringComparison.OrdinalIgnoreCase));

        IReadOnlyList<CompanyDomain> result = query.ToList();
        return Task.FromResult(result);
    }
}
