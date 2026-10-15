namespace Verus.Domain.CompanyBoundedContext.Services;

public static class WebsiteDomainResolver
{
    private static readonly HashSet<string> SecondLevelSuffixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "co.uk", "org.uk", "ac.uk", "gov.uk", "co.nz", "co.jp", "com.br",
        "com.au", "com.mx", "co.in", "co.za", "com.ar", "com.co",
    };

    public static string GetRegistrableDomain(Uri uri)
    {
        var host = uri.Host;
        if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            host = host[4..];

        var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 2)
            return host;

        var lastTwo = string.Join('.', parts[^2], parts[^1]);
        return SecondLevelSuffixes.Contains(lastTwo)
            ? string.Join('.', parts[^3..])
            : lastTwo;
    }

    public static string GetRegistrableLabel(Uri uri)
    {
        var registrable = GetRegistrableDomain(uri);
        var separator = registrable.IndexOf('.');
        return separator > 0 ? registrable[..separator] : registrable;
    }
}
