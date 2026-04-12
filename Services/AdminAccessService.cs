using FutureTech.StudentManagement.Web.Options;
using Microsoft.Extensions.Options;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class AdminAccessService : IAdminAccessService
{
    private readonly HashSet<string> _allowedEmails;
    private readonly HashSet<string> _allowedDomains;

    public AdminAccessService(IOptions<AdminAccessOptions> options)
    {
        _allowedEmails = options.Value.AllowedEmails
            .Concat(SplitCsv(options.Value.AllowedEmailsCsv))
            .Select(NormalizeEmail)
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .ToHashSet();

        _allowedDomains = options.Value.AllowedDomains
            .Concat(SplitCsv(options.Value.AllowedDomainsCsv))
            .Select(NormalizeDomain)
            .Where(domain => !string.IsNullOrWhiteSpace(domain))
            .ToHashSet();
    }

    public bool IsAdminEmail(string? emailAddress)
    {
        var normalizedEmail = NormalizeEmail(emailAddress);

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return false;
        }

        if (_allowedEmails.Contains(normalizedEmail))
        {
            return true;
        }

        var atIndex = normalizedEmail.LastIndexOf('@');

        if (atIndex < 0 || atIndex == normalizedEmail.Length - 1)
        {
            return false;
        }

        var domain = normalizedEmail[(atIndex + 1)..];
        return _allowedDomains.Contains(domain);
    }

    private static string NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email)
            ? string.Empty
            : email.Trim().ToLowerInvariant();
    }

    private static string NormalizeDomain(string? domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
        {
            return string.Empty;
        }

        var normalized = domain.Trim().ToLowerInvariant();
        return normalized.StartsWith('@') ? normalized[1..] : normalized;
    }

    private static IEnumerable<string> SplitCsv(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return [];
        }

        return csv.Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
