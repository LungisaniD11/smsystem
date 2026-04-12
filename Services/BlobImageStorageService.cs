using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using FutureTech.StudentManagement.Web.Options;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class BlobImageStorageService : IImageStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly BlobStorageOptions _options;

    public BlobImageStorageService(IOptions<BlobStorageOptions> options)
    {
        _options = options.Value;
        var serviceClient = new BlobServiceClient(_options.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(_options.ContainerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<(string BlobName, string BlobUrl)> UploadProfileImageAsync(string studentId, IFormFile file, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blobName = $"{studentId}-{Guid.NewGuid():N}{extension}";

        await using var sourceStream = file.OpenReadStream();
        using var image = await Image.LoadAsync(sourceStream, cancellationToken);

        image.Mutate(context =>
            context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(_options.ResizeWidth, _options.ResizeHeight)
            }));

        await using var outputStream = new MemoryStream();

        if (extension == ".png")
        {
            await image.SaveAsync(outputStream, new PngEncoder(), cancellationToken);
        }
        else
        {
            await image.SaveAsync(outputStream, new JpegEncoder { Quality = 85 }, cancellationToken);
        }

        outputStream.Position = 0;

        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(outputStream, overwrite: true, cancellationToken: cancellationToken);

        return (blobName, blobClient.Uri.ToString());
    }

    public async Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return;
        }

        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public string BuildReadSasUrl(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return string.Empty;
        }

        var blobClient = _containerClient.GetBlobClient(blobName);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_options.SasExpiryMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri.ToString();
    }
}
