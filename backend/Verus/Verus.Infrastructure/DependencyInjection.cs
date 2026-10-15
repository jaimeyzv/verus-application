using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Verus.Application.Contracts.Relevance;
using Verus.Domain.CompanyBoundedContext.Repositories;
using Verus.Infrastructure.Persistence.InMemory;
using Verus.Infrastructure.Relevance;

namespace Verus.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RelevanceOptions>(configuration.GetSection(RelevanceOptions.SectionName));

        services.AddSingleton<ICompanyRepository, InMemoryCompanyRepository>();
        services.AddSingleton<IRelevanceEvaluator, RelevanceEvaluator>();

        return services;
    }
}
