namespace FutureTech.StudentManagement.Web.ViewModels;

public sealed class StudentListItemViewModel
{
    public string Id { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string MobileNumber { get; init; } = string.Empty;

    public string EnrolmentStatus { get; init; } = string.Empty;

    public bool IsSoftDeleted { get; init; }

    public string ProfileImageSasUrl { get; init; } = string.Empty;
}
