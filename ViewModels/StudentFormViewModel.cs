using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FutureTech.StudentManagement.Web.ViewModels;

public sealed class StudentFormViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(25)]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    [RegularExpression("Active|Inactive", ErrorMessage = "Enrolment status must be Active or Inactive.")]
    public string EnrolmentStatus { get; set; } = "Active";

    public IFormFile? ProfileImage { get; set; }

    public string ExistingProfileImageBlobName { get; set; } = string.Empty;

    public string ExistingProfileImageSasUrl { get; set; } = string.Empty;
}
