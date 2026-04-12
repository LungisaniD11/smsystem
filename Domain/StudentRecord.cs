using System.Text.Json.Serialization;

namespace FutureTech.StudentManagement.Web.Domain;

public sealed class StudentRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("mobileNumber")]
    public string MobileNumber { get; set; } = string.Empty;

    [JsonPropertyName("enrolmentStatus")]
    public string EnrolmentStatus { get; set; } = "Active";

    [JsonPropertyName("profileImageUrl")]
    public string ProfileImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("profileImageBlobName")]
    public string ProfileImageBlobName { get; set; } = string.Empty;

    [JsonPropertyName("isSoftDeleted")]
    public bool IsSoftDeleted { get; set; }

    [JsonPropertyName("createdAtUtc")]
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("updatedAtUtc")]
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
