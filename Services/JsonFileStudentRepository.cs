using System.Text.Json;
using FutureTech.StudentManagement.Web.Domain;
using Microsoft.AspNetCore.Hosting;

namespace FutureTech.StudentManagement.Web.Services;

/// <summary>
/// File-backed student repository for development.
/// Persists all student records to App_Data/students.json so data survives app restarts.
/// </summary>
public sealed class JsonFileStudentRepository : IStudentRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private Dictionary<string, StudentRecord>? _cache;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonFileStudentRepository(IWebHostEnvironment env)
    {
        var dataFolder = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataFolder);
        _filePath = Path.Combine(dataFolder, "students.json");
    }

    private async Task<Dictionary<string, StudentRecord>> LoadAsync()
    {
        if (_cache is not null) return _cache;

        if (File.Exists(_filePath))
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var records = JsonSerializer.Deserialize<List<StudentRecord>>(json, JsonOptions) ?? [];
            _cache = records.ToDictionary(r => r.Id, StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            _cache = new Dictionary<string, StudentRecord>(StringComparer.OrdinalIgnoreCase);
        }

        return _cache;
    }

    private async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_cache!.Values.ToList(), JsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var data = await LoadAsync();
            data.TryGetValue(id, out var student);
            return student is null ? null : Clone(student);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, pageSize);

        await _lock.WaitAsync(cancellationToken);
        try
        {
            var data = await LoadAsync();
            var normalized = query?.Trim();
            var filtered = data.Values
                .Where(s => string.IsNullOrWhiteSpace(normalized)
                    || s.FirstName.Contains(normalized, StringComparison.OrdinalIgnoreCase)
                    || s.LastName.Contains(normalized, StringComparison.OrdinalIgnoreCase)
                    || s.Id.Contains(normalized, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(s => s.UpdatedAtUtc)
                .ToList();

            var totalCount = filtered.Count;
            var items = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(Clone)
                .ToList();

            return new PagedResult<StudentRecord>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpsertAsync(StudentRecord student, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var data = await LoadAsync();
            data[student.Id] = Clone(student);
            await SaveAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            var data = await LoadAsync();
            data.Remove(id);
            await SaveAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    private static StudentRecord Clone(StudentRecord s) => new()
    {
        Id = s.Id,
        FirstName = s.FirstName,
        LastName = s.LastName,
        Email = s.Email,
        MobileNumber = s.MobileNumber,
        EnrolmentStatus = s.EnrolmentStatus,
        ProfileImageUrl = s.ProfileImageUrl,
        ProfileImageBlobName = s.ProfileImageBlobName,
        IsSoftDeleted = s.IsSoftDeleted,
        CreatedAtUtc = s.CreatedAtUtc,
        UpdatedAtUtc = s.UpdatedAtUtc
    };
}
