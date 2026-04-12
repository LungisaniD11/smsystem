using FutureTech.StudentManagement.Web.Domain;
using FutureTech.StudentManagement.Web.ViewModels;

namespace FutureTech.StudentManagement.Web.Services;

public interface IStudentService
{
    Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<StudentRecord> CreateAsync(StudentFormViewModel model, CancellationToken cancellationToken = default);

    Task<StudentRecord?> UpdateAsync(StudentFormViewModel model, CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<bool> PermanentDeleteAsync(string id, CancellationToken cancellationToken = default);

    string BuildSasUrl(string blobName);
}
