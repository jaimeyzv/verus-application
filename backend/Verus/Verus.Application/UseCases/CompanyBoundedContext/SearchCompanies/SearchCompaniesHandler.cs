using MediatR;
using Verus.Application.Contracts.Relevance;
using Verus.Application.UseCases.CompanyBoundedContext.Common;
using Verus.Domain.CompanyBoundedContext.Repositories;

namespace Verus.Application.UseCases.CompanyBoundedContext.SearchCompanies;

internal sealed class SearchCompaniesHandler(
    ICompanyRepository repository,
    IRelevanceEvaluator relevanceEvaluator)
    : IRequestHandler<SearchCompaniesQuery, IReadOnlyList<CompanyResponse>>
{
    public async Task<IReadOnlyList<CompanyResponse>> Handle(
        SearchCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var candidates = await repository.SearchAsync(request.Name, request.Domain, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.Term))
        {
            return candidates
                .OrderBy(company => company.Name, StringComparer.OrdinalIgnoreCase)
                .Select(company => company.ToResponse())
                .ToList();
        }

        return candidates
            .Select(company => new
            {
                Company = company,
                Score = relevanceEvaluator.ScoreSearch(request.Term!, company.Name, company.WebsiteDomain),
            })
            .Where(scored => scored.Score > 0)
            .OrderByDescending(scored => scored.Score)
            .ThenBy(scored => scored.Company.Name, StringComparer.OrdinalIgnoreCase)
            .Select(scored => scored.Company.ToResponse())
            .ToList();
    }
}
