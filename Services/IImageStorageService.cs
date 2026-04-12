using Microsoft.AspNetCore.Http;

namespace FutureTech.StudentManagement.Web.Services;

public interface IImageStorageService
{
    Task<(string BlobName, string BlobUrl)> UploadProfileImageAsync(string studentId, IFormFile file, CancellationToken cancellationToken = default);

    Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default);

    string BuildReadSasUrl(string blobName);
}
