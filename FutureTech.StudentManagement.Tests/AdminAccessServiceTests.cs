using FutureTech.StudentManagement.Web.Options;
using FutureTech.StudentManagement.Web.Services;
using Microsoft.Extensions.Options;

namespace FutureTech.StudentManagement.Tests;

public class AdminAccessServiceTests
{
    [Fact]
    public void IsAdminEmail_ReturnsTrue_ForAllowedAddress()
    {
        var options = Options.Create(new AdminAccessOptions
        {
            AllowedEmails = ["admin@futuretech.ac.za"]
        });

        var service = new AdminAccessService(options);

        var result = service.IsAdminEmail("admin@futuretech.ac.za");

        Assert.True(result);
    }

    [Fact]
    public void IsAdminEmail_ReturnsFalse_ForUnknownAddress()
    {
        var options = Options.Create(new AdminAccessOptions
        {
            AllowedEmails = ["admin@futuretech.ac.za"]
        });

        var service = new AdminAccessService(options);

        var result = service.IsAdminEmail("student@futuretech.ac.za");

        Assert.False(result);
    }

    [Fact]
    public void IsAdminEmail_ReturnsTrue_ForAllowedDomain()
    {
        var options = Options.Create(new AdminAccessOptions
        {
            AllowedDomains = ["futuretech.ac.za"]
        });

        var service = new AdminAccessService(options);

        var result = service.IsAdminEmail("any.admin@futuretech.ac.za");

        Assert.True(result);
    }

    [Fact]
    public void IsAdminEmail_ReturnsTrue_ForCsvConfiguredValues()
    {
        var options = Options.Create(new AdminAccessOptions
        {
            AllowedEmailsCsv = "admin.one@example.com;admin.two@example.com",
            AllowedDomainsCsv = "futuretech.ac.za,admins.futuretech.ac.za"
        });

        var service = new AdminAccessService(options);

        Assert.True(service.IsAdminEmail("admin.one@example.com"));
        Assert.True(service.IsAdminEmail("user@admins.futuretech.ac.za"));
    }
}
