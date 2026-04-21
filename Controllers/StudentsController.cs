using FutureTech.StudentManagement.Web.ViewModels;
using FutureTech.StudentManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureTech.StudentManagement.Web.Controllers;

[Authorize(Policy = "AdminOnly")]
public sealed class StudentsController : Controller
{
    private const int DefaultPageSize = 10;
    private const int DashboardSampleSize = 2000;
    private static int _permanentlyDeletedCount;
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var dashboardResult = await _studentService.SearchAsync(null, 1, DashboardSampleSize, cancellationToken);

        var dashboardItems = dashboardResult.Items;
        var activeCount = dashboardItems.Count(student =>
            student.EnrolmentStatus.Equals("Active", StringComparison.OrdinalIgnoreCase)
            && !student.IsSoftDeleted);
        var inactiveCount = dashboardItems.Count(student =>
            student.EnrolmentStatus.Equals("Inactive", StringComparison.OrdinalIgnoreCase)
            || student.IsSoftDeleted);
        var softDeletedCount = dashboardItems.Count(student => student.IsSoftDeleted);
        var withImageCount = dashboardItems.Count(student => !string.IsNullOrWhiteSpace(student.ProfileImageBlobName));
        var newThisWeekCount = dashboardItems.Count(student => student.CreatedAtUtc >= DateTimeOffset.UtcNow.AddDays(-7));

        var viewModel = new StudentIndexViewModel
        {
            SearchQuery = string.Empty,
            PageNumber = 1,
            PageSize = Math.Max(1, dashboardItems.Count),
            TotalCount = dashboardItems.Count,
            ActiveCount = activeCount,
            InactiveCount = inactiveCount,
            SoftDeletedCount = softDeletedCount,
            PermanentlyDeletedCount = Volatile.Read(ref _permanentlyDeletedCount),
            WithImageCount = withImageCount,
            NewThisWeekCount = newThisWeekCount,
            Students = dashboardItems
                .OrderByDescending(student => student.CreatedAtUtc)
                .Take(12)
                .Select(student => new StudentListItemViewModel
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    MobileNumber = student.MobileNumber,
                    EnrolmentStatus = student.EnrolmentStatus,
                    IsSoftDeleted = student.IsSoftDeleted,
                    ProfileImageSasUrl = _studentService.BuildSasUrl(student.ProfileImageBlobName),
                    DateEnrolledUtc = student.CreatedAtUtc
                })
                .ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EnrolledStudents(string? query, int page = 1, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        var result = await _studentService.SearchAsync(query, page, DefaultPageSize, cancellationToken);

        var viewModel = new StudentIndexViewModel
        {
            SearchQuery = query ?? string.Empty,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            ActiveCount = 0,
            InactiveCount = 0,
            SoftDeletedCount = 0,
            PermanentlyDeletedCount = 0,
            WithImageCount = 0,
            NewThisWeekCount = 0,
            Students = result.Items.Select(student => new StudentListItemViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                MobileNumber = student.MobileNumber,
                EnrolmentStatus = student.EnrolmentStatus,
                IsSoftDeleted = student.IsSoftDeleted,
                ProfileImageSasUrl = _studentService.BuildSasUrl(student.ProfileImageBlobName),
                DateEnrolledUtc = student.CreatedAtUtc
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new StudentFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _studentService.CreateAsync(model, cancellationToken);
            TempData["Success"] = "Student record created successfully.";
            return RedirectToAction(nameof(EnrolledStudents));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        if (student is null)
        {
            return NotFound();
        }

        var viewModel = new StudentFormViewModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            MobileNumber = student.MobileNumber,
            EnrolmentStatus = student.EnrolmentStatus,
            ExistingProfileImageBlobName = student.ProfileImageBlobName,
            ExistingProfileImageSasUrl = _studentService.BuildSasUrl(student.ProfileImageBlobName)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.ExistingProfileImageSasUrl = string.IsNullOrWhiteSpace(model.ExistingProfileImageBlobName)
                ? string.Empty
                : _studentService.BuildSasUrl(model.ExistingProfileImageBlobName);
            return View(model);
        }

        try
        {
            var updated = await _studentService.UpdateAsync(model, cancellationToken);
            if (updated is null)
            {
                return NotFound();
            }

            TempData["Success"] = "Student record updated.";
            return RedirectToAction(nameof(EnrolledStudents));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.ExistingProfileImageSasUrl = string.IsNullOrWhiteSpace(model.ExistingProfileImageBlobName)
                ? string.Empty
                : _studentService.BuildSasUrl(model.ExistingProfileImageBlobName);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(string id, CancellationToken cancellationToken)
    {
        var success = await _studentService.SoftDeleteAsync(id, cancellationToken);
        TempData["Success"] = success
            ? "Student has been marked as inactive."
            : "Student record was not found.";
        return RedirectToAction(nameof(EnrolledStudents));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PermanentDelete(string id, CancellationToken cancellationToken)
    {
        var success = await _studentService.PermanentDeleteAsync(id, cancellationToken);
        if (success)
        {
            Interlocked.Increment(ref _permanentlyDeletedCount);
        }

        TempData["Success"] = success
            ? "Student record was permanently deleted."
            : "Student record was not found.";

        return RedirectToAction(nameof(EnrolledStudents));
    }
}
