using FutureTech.StudentManagement.Web.Domain;
using FutureTech.StudentManagement.Web.ViewModels;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class StudentService : IStudentService
{
    private readonly IStudentRepository _repository;
    private readonly IImageStorageService _imageStorageService;
    private readonly ImageValidationService _imageValidationService;

    public StudentService(
        IStudentRepository repository,
        IImageStorageService imageStorageService,
        ImageValidationService imageValidationService)
    {
        _repository = repository;
        _imageStorageService = imageStorageService;
        _imageValidationService = imageValidationService;
    }

    public Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return _repository.SearchAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<StudentRecord> CreateAsync(StudentFormViewModel model, CancellationToken cancellationToken = default)
    {
        var validation = _imageValidationService.Validate(model.ProfileImage, required: true);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Error);
        }

        var studentId = $"student-{Guid.NewGuid():N}";
        var uploadResult = await _imageStorageService.UploadProfileImageAsync(studentId, model.ProfileImage!, cancellationToken);

        var student = new StudentRecord
        {
            Id = studentId,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = model.Email.Trim(),
            MobileNumber = model.MobileNumber.Trim(),
            EnrolmentStatus = model.EnrolmentStatus,
            ProfileImageUrl = uploadResult.BlobUrl,
            ProfileImageBlobName = uploadResult.BlobName,
            IsSoftDeleted = false,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        await _repository.UpsertAsync(student, cancellationToken);
        return student;
    }

    public async Task<StudentRecord?> UpdateAsync(StudentFormViewModel model, CancellationToken cancellationToken = default)
    {
        var existingStudent = await _repository.GetByIdAsync(model.Id, cancellationToken);
        if (existingStudent is null)
        {
            return null;
        }

        var validation = _imageValidationService.Validate(model.ProfileImage, required: false);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException(validation.Error);
        }

        if (model.ProfileImage is not null)
        {
            var uploadResult = await _imageStorageService.UploadProfileImageAsync(existingStudent.Id, model.ProfileImage, cancellationToken);
            await _imageStorageService.DeleteImageAsync(existingStudent.ProfileImageBlobName, cancellationToken);
            existingStudent.ProfileImageBlobName = uploadResult.BlobName;
            existingStudent.ProfileImageUrl = uploadResult.BlobUrl;
        }

        existingStudent.FirstName = model.FirstName.Trim();
        existingStudent.LastName = model.LastName.Trim();
        existingStudent.Email = model.Email.Trim();
        existingStudent.MobileNumber = model.MobileNumber.Trim();
        existingStudent.EnrolmentStatus = model.EnrolmentStatus;
        if (model.EnrolmentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
        {
            existingStudent.IsSoftDeleted = false;
        }
        else if (model.EnrolmentStatus.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
        {
            existingStudent.IsSoftDeleted = true;
        }

        existingStudent.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _repository.UpsertAsync(existingStudent, cancellationToken);
        return existingStudent;
    }

    public async Task<bool> SoftDeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existingStudent = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingStudent is null)
        {
            return false;
        }

        existingStudent.EnrolmentStatus = "Inactive";
        existingStudent.IsSoftDeleted = true;
        existingStudent.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _repository.UpsertAsync(existingStudent, cancellationToken);
        return true;
    }

    public async Task<bool> PermanentDeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existingStudent = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingStudent is null)
        {
            return false;
        }

        await _imageStorageService.DeleteImageAsync(existingStudent.ProfileImageBlobName, cancellationToken);
        await _repository.DeleteAsync(existingStudent.Id, cancellationToken);

        return true;
    }

    public string BuildSasUrl(string blobName)
    {
        return _imageStorageService.BuildReadSasUrl(blobName);
    }
}
