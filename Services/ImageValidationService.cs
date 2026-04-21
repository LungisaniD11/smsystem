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

    // JPEG magic bytes: FF D8 FF (3 bytes)
    private static readonly byte[] JpegMagic = [0xFF, 0xD8, 0xFF];

    // PNG magic bytes: 89 50 4E 47 0D 0A 1A 0A (8 bytes)
    private static readonly byte[] PngMagic = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

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

        // Validate actual file content via magic bytes — prevents extension/content-type spoofing
        if (!HasValidMagicBytes(file))
        {
            return (false, "File content does not match a valid JPEG or PNG image.");
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// Reads the first 8 bytes of the upload and verifies the binary signature
    /// matches either a JPEG (FF D8 FF) or PNG (89 50 4E 47 0D 0A 1A 0A) file.
    /// </summary>
    private static bool HasValidMagicBytes(IFormFile file)
    {
        Span<byte> buffer = stackalloc byte[8];
        using var stream = file.OpenReadStream();
        var bytesRead = stream.Read(buffer);

        if (bytesRead >= 3
            && buffer[0] == JpegMagic[0]
            && buffer[1] == JpegMagic[1]
            && buffer[2] == JpegMagic[2])
        {
            return true;
        }

        if (bytesRead >= 8
            && buffer[0] == PngMagic[0]
            && buffer[1] == PngMagic[1]
            && buffer[2] == PngMagic[2]
            && buffer[3] == PngMagic[3]
            && buffer[4] == PngMagic[4]
            && buffer[5] == PngMagic[5]
            && buffer[6] == PngMagic[6]
            && buffer[7] == PngMagic[7])
        {
            return true;
        }

        return false;
    }
}
