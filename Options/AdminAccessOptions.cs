namespace FutureTech.StudentManagement.Web.Options;

public sealed class AdminAccessOptions
{
    public const string SectionName = "Authentication:AdminAccess";

    public List<string> AllowedEmails { get; set; } = [];

    public string? AllowedEmailsCsv { get; set; }

    public List<string> AllowedDomains { get; set; } = [];

    public string? AllowedDomainsCsv { get; set; }
}
