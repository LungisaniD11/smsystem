namespace FutureTech.StudentManagement.Web.Services;

public interface IAdminAccessService
{
    bool IsAdminEmail(string? emailAddress);
}
