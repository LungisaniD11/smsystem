using FutureTech.StudentManagement.Web.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class ImageValidationService
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png"
    ];

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];

    private readonly BlobStorageOptions _blobOptions;

    public ImageValidationService(IOptions<BlobStorageOptions> blobOptions)
    {
        _blobOptions = blobOptions.Value;
    }

    public (bool IsValid, string Error) Validate(IFormFile? file, bool required)
    {
        if (file is null)
        {
            return required ? (false, "Profile image is required.") : (true, string.Empty);
        }

        if (file.Length == 0)
        {
            return (false, "Uploaded image is empty.");
        }

        var maxBytes = _blobOptions.MaxUploadSizeMb * 1024L * 1024L;
        if (file.Length > maxBytes)
        {
            return (false, $"Maximum image size is {_blobOptions.MaxUploadSizeMb} MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return (false, "Only JPEG and PNG images are allowed.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return (false, "Invalid image content type.");
        }

        return (true, string.Empty);
    }
}
