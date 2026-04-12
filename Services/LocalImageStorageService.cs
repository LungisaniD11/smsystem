using FutureTech.StudentManagement.Web.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class LocalImageStorageService : IImageStorageService
{
    private readonly BlobStorageOptions _options;
    private readonly string _uploadsDirectory;

    public LocalImageStorageService(IWebHostEnvironment environment, IOptions<BlobStorageOptions> options)
    {
        _options = options.Value;

        var webRootPath = string.IsNullOrWhiteSpace(environment.WebRootPath)
            ? Path.Combine(environment.ContentRootPath, "wwwroot")
            : environment.WebRootPath;

        _uploadsDirectory = Path.Combine(webRootPath, "uploads");
        Directory.CreateDirectory(_uploadsDirectory);
    }

    public async Task<(string BlobName, string BlobUrl)> UploadProfileImageAsync(string studentId, IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var blobName = $"{studentId}-{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(_uploadsDirectory, blobName);

        await using var sourceStream = file.OpenReadStream();
        using var image = await Image.LoadAsync(sourceStream, cancellationToken);

        image.Mutate(context =>
            context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(_options.ResizeWidth, _options.ResizeHeight)
            }));

        await using var outputStream = File.Create(absolutePath);

        if (extension == ".png")
        {
            await image.SaveAsync(outputStream, new PngEncoder(), cancellationToken);
        }
        else
        {
            await image.SaveAsync(outputStream, new JpegEncoder { Quality = 85 }, cancellationToken);
        }

        return (blobName, BuildReadSasUrl(blobName));
    }

    public Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return Task.CompletedTask;
        }

        var absolutePath = Path.Combine(_uploadsDirectory, blobName);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }

    public string BuildReadSasUrl(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return string.Empty;
        }

        return $"/uploads/{blobName}";
    }
}