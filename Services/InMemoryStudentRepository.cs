using FutureTech.StudentManagement.Web.Domain;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly Dictionary<string, StudentRecord> _students = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    public Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            _students.TryGetValue(id, out var student);
            return Task.FromResult(student is null ? null : Clone(student));
        }
    }

    public Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, pageSize);

        lock (_sync)
        {
            var normalized = query?.Trim();
            var filtered = _students.Values
                .Where(student => string.IsNullOrWhiteSpace(normalized)
                    || student.FirstName.Contains(normalized, StringComparison.OrdinalIgnoreCase)
                    || student.LastName.Contains(normalized, StringComparison.OrdinalIgnoreCase)
                    || student.Id.Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(student => student.UpdatedAtUtc)
                .ToList();

            var totalCount = filtered.Count;
            var items = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(Clone)
                .ToList();

            var result = new PagedResult<StudentRecord>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Task.FromResult(result);
        }
    }

    public Task UpsertAsync(StudentRecord student, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            _students[student.Id] = Clone(student);
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            _students.Remove(id);
            return Task.CompletedTask;
        }
    }

    private static StudentRecord Clone(StudentRecord student)
    {
        return new StudentRecord
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            MobileNumber = student.MobileNumber,
            EnrolmentStatus = student.EnrolmentStatus,
            ProfileImageUrl = student.ProfileImageUrl,
            ProfileImageBlobName = student.ProfileImageBlobName,
            IsSoftDeleted = student.IsSoftDeleted,
            CreatedAtUtc = student.CreatedAtUtc,
            UpdatedAtUtc = student.UpdatedAtUtc
        };
    }
}