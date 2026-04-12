using FutureTech.StudentManagement.Web.Domain;

namespace FutureTech.StudentManagement.Web.Services;

public interface IStudentRepository
{
    Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task UpsertAsync(StudentRecord student, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
