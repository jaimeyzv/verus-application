using Verus.Domain.CompanyBoundedContext.Services;

namespace Verus.Domain.CompanyBoundedContext.Entities;

public sealed class CompanyDomain
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Website { get; private set; }
    public string WebsiteDomain { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private CompanyDomain()
    {
        Name = string.Empty;
        Website = string.Empty;
        WebsiteDomain = string.Empty;
    }

    public CompanyDomain(Guid id, string name, string website, string websiteDomain, DateTime createdAtUtc)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Website = website ?? throw new ArgumentNullException(nameof(website));
        WebsiteDomain = websiteDomain ?? throw new ArgumentNullException(nameof(websiteDomain));
        CreatedAtUtc = createdAtUtc;
    }

    public static CompanyDomain CreateNew(string name, string website)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name is required.", nameof(name));

        if (!Uri.TryCreate(website, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Website must be a valid, well-formed absolute URL.", nameof(website));
        }

        return new CompanyDomain(
            Guid.CreateVersion7(),
            name.Trim(),
            uri.ToString(),
            WebsiteDomainResolver.GetRegistrableDomain(uri),
            DateTime.UtcNow);
    }
}
