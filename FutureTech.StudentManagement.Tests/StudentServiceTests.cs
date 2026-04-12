using FutureTech.StudentManagement.Web.Domain;
using FutureTech.StudentManagement.Web.Options;
using FutureTech.StudentManagement.Web.Services;
using FutureTech.StudentManagement.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FutureTech.StudentManagement.Tests;

public class StudentServiceTests
{
    [Fact]
    public async Task CreateAsync_AddsStudentRecord_WithImageData()
    {
        var repository = new InMemoryStudentRepository();
        var imageStorage = new FakeImageStorageService();
        var validator = BuildImageValidator();
        var service = new StudentService(repository, imageStorage, validator);

        var model = new StudentFormViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            MobileNumber = "+27123456789",
            EnrolmentStatus = "Active",
            ProfileImage = BuildImage("avatar.jpg", "image/jpeg")
        };

        var created = await service.CreateAsync(model);

        Assert.StartsWith("student-", created.Id, StringComparison.Ordinal);
        Assert.Equal("John", created.FirstName);
        Assert.Equal(created.Id, imageStorage.LastUploadedStudentId);
        Assert.Single(repository.Students);
    }

    [Fact]
    public async Task UpdateAsync_ChangesExistingStudentData()
    {
        var repository = new InMemoryStudentRepository();
        var imageStorage = new FakeImageStorageService();
        var validator = BuildImageValidator();
        var service = new StudentService(repository, imageStorage, validator);

        var existing = new StudentRecord
        {
            Id = "student-001",
            FirstName = "Mary",
            LastName = "Jones",
            Email = "mary@example.com",
            MobileNumber = "+27110000000",
            EnrolmentStatus = "Active",
            ProfileImageBlobName = "student-001-old.jpg",
            ProfileImageUrl = "https://blob/old.jpg"
        };

        repository.Students.Add(existing);

        var model = new StudentFormViewModel
        {
            Id = existing.Id,
            FirstName = "Maria",
            LastName = "Jones",
            Email = "maria@example.com",
            MobileNumber = "+27119999999",
            EnrolmentStatus = "Active"
        };

        var updated = await service.UpdateAsync(model);

        Assert.NotNull(updated);
        Assert.Equal("Maria", updated!.FirstName);
        Assert.Equal("maria@example.com", updated.Email);
    }

    [Fact]
    public async Task SoftDeleteAsync_MarksStudentInactive()
    {
        var repository = new InMemoryStudentRepository();
        var imageStorage = new FakeImageStorageService();
        var validator = BuildImageValidator();
        var service = new StudentService(repository, imageStorage, validator);

        repository.Students.Add(new StudentRecord
        {
            Id = "student-100",
            FirstName = "Neo",
            LastName = "Kim",
            Email = "neo@example.com",
            MobileNumber = "+27115556677",
            EnrolmentStatus = "Active",
            ProfileImageBlobName = "student-100.jpg",
            ProfileImageUrl = "https://blob/student-100.jpg"
        });

        var result = await service.SoftDeleteAsync("student-100");

        Assert.True(result);
        Assert.Equal("Inactive", repository.Students[0].EnrolmentStatus);
        Assert.True(repository.Students[0].IsSoftDeleted);
    }

    [Fact]
    public async Task PermanentDeleteAsync_RemovesStudentAndBlob()
    {
        var repository = new InMemoryStudentRepository();
        var imageStorage = new FakeImageStorageService();
        var validator = BuildImageValidator();
        var service = new StudentService(repository, imageStorage, validator);

        repository.Students.Add(new StudentRecord
        {
            Id = "student-101",
            FirstName = "Amy",
            LastName = "Rex",
            Email = "amy@example.com",
            MobileNumber = "+27111112222",
            EnrolmentStatus = "Active",
            ProfileImageBlobName = "student-101.jpg",
            ProfileImageUrl = "https://blob/student-101.jpg"
        });

        var result = await service.PermanentDeleteAsync("student-101");

        Assert.True(result);
        Assert.Empty(repository.Students);
        Assert.Equal("student-101.jpg", imageStorage.LastDeletedBlobName);
    }

    private static ImageValidationService BuildImageValidator()
    {
        var options = Options.Create(new BlobStorageOptions
        {
            MaxUploadSizeMb = 5
        });

        return new ImageValidationService(options);
    }

    private static IFormFile BuildImage(string fileName, string contentType)
    {
        var bytes = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70, 0, 1, 1, 1 };
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "ProfileImage", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private sealed class InMemoryStudentRepository : IStudentRepository
    {
        public List<StudentRecord> Students { get; } = [];

        public Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Students.FirstOrDefault(student => student.Id == id));
        }

        public Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var filtered = string.IsNullOrWhiteSpace(query)
                ? Students
                : Students.Where(s => s.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || s.LastName.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || s.Id.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return Task.FromResult(new PagedResult<StudentRecord>
            {
                Items = filtered,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = filtered.Count
            });
        }

        public Task UpsertAsync(StudentRecord student, CancellationToken cancellationToken = default)
        {
            var existingIndex = Students.FindIndex(s => s.Id == student.Id);
            if (existingIndex >= 0)
            {
                Students[existingIndex] = student;
            }
            else
            {
                Students.Add(student);
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            Students.RemoveAll(student => student.Id == id);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeImageStorageService : IImageStorageService
    {
        public string LastDeletedBlobName { get; private set; } = string.Empty;

        public string LastUploadedStudentId { get; private set; } = string.Empty;

        public Task<(string BlobName, string BlobUrl)> UploadProfileImageAsync(string studentId, IFormFile file, CancellationToken cancellationToken = default)
        {
            LastUploadedStudentId = studentId;
            return Task.FromResult(($"{studentId}.jpg", $"https://blob/{studentId}.jpg"));
        }

        public Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default)
        {
            LastDeletedBlobName = blobName;
            return Task.CompletedTask;
        }

        public string BuildReadSasUrl(string blobName)
        {
            return $"https://blob/{blobName}?sig=token";
        }
    }
}
