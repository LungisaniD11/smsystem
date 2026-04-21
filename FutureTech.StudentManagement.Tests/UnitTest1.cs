using FutureTech.StudentManagement.Web.Options;
using FutureTech.StudentManagement.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FutureTech.StudentManagement.Tests;

/// <summary>
/// Unit tests for ImageValidationService — verifies that secure file handling
/// rules (magic bytes, extension, content-type, size) are correctly enforced.
/// </summary>
public class ImageValidationServiceTests
{
    // ── JPEG magic: FF D8 FF ──────────────────────────────────────────────────

    [Fact]
    public void Validate_AcceptsValidJpeg()
    {
        var service = BuildService(maxMb: 5);
        var file = BuildFile("photo.jpg", "image/jpeg", JpegBytes());

        var (isValid, error) = service.Validate(file, required: true);

        Assert.True(isValid);
        Assert.Empty(error);
    }

    // ── PNG magic: 89 50 4E 47 0D 0A 1A 0A ───────────────────────────────────

    [Fact]
    public void Validate_AcceptsValidPng()
    {
        var service = BuildService(maxMb: 5);
        var file = BuildFile("photo.png", "image/png", PngBytes());

        var (isValid, error) = service.Validate(file, required: true);

        Assert.True(isValid);
        Assert.Empty(error);
    }

    // ── Magic bytes spoofing ──────────────────────────────────────────────────

    [Fact]
    public void Validate_RejectsFileWithWrongMagicBytes()
    {
        var service = BuildService(maxMb: 5);
        // Looks like a JPEG by name/content-type but has PDF magic bytes
        var fakeBytes = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x00, 0x00, 0x00, 0x00 }; // %PDF....
        var file = BuildFile("malicious.jpg", "image/jpeg", fakeBytes);

        var (isValid, error) = service.Validate(file, required: true);

        Assert.False(isValid);
        Assert.Contains("content does not match", error, StringComparison.OrdinalIgnoreCase);
    }

    // ── Extension check ───────────────────────────────────────────────────────

    [Fact]
    public void Validate_RejectsDisallowedExtension()
    {
        var service = BuildService(maxMb: 5);
        var file = BuildFile("script.gif", "image/jpeg", JpegBytes());

        var (isValid, error) = service.Validate(file, required: true);

        Assert.False(isValid);
        Assert.Contains("JPEG and PNG", error, StringComparison.OrdinalIgnoreCase);
    }

    // ── Content-type check ────────────────────────────────────────────────────

    [Fact]
    public void Validate_RejectsInvalidContentType()
    {
        var service = BuildService(maxMb: 5);
        var file = BuildFile("photo.jpg", "application/octet-stream", JpegBytes());

        var (isValid, error) = service.Validate(file, required: true);

        Assert.False(isValid);
        Assert.Contains("content type", error, StringComparison.OrdinalIgnoreCase);
    }

    // ── File size check ───────────────────────────────────────────────────────

    [Fact]
    public void Validate_RejectsOversizedFile()
    {
        var service = BuildService(maxMb: 1);
        // Build a 2 MB payload (starts with JPEG magic bytes)
        var bigBytes = new byte[2 * 1024 * 1024];
        JpegBytes().CopyTo(bigBytes, 0);
        var file = BuildFile("large.jpg", "image/jpeg", bigBytes);

        var (isValid, error) = service.Validate(file, required: true);

        Assert.False(isValid);
        Assert.Contains("1 MB", error, StringComparison.OrdinalIgnoreCase);
    }

    // ── Required / optional ───────────────────────────────────────────────────

    [Fact]
    public void Validate_ReturnsValid_WhenFileNullAndNotRequired()
    {
        var service = BuildService(maxMb: 5);

        var (isValid, error) = service.Validate(null, required: false);

        Assert.True(isValid);
        Assert.Empty(error);
    }

    [Fact]
    public void Validate_ReturnsInvalid_WhenFileNullAndRequired()
    {
        var service = BuildService(maxMb: 5);

        var (isValid, error) = service.Validate(null, required: true);

        Assert.False(isValid);
        Assert.NotEmpty(error);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ImageValidationService BuildService(int maxMb) =>
        new(Options.Create(new BlobStorageOptions { MaxUploadSizeMb = maxMb }));

    private static IFormFile BuildFile(string fileName, string contentType, byte[] content)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "ProfileImage", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static byte[] JpegBytes() =>
        [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01];

    private static byte[] PngBytes() =>
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D];
}
