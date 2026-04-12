namespace FutureTech.StudentManagement.Web.ViewModels;

public sealed class StudentIndexViewModel
{
    public required IReadOnlyList<StudentListItemViewModel> Students { get; init; }

    public required string SearchQuery { get; init; }

    public required int PageNumber { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public required int ActiveCount { get; init; }

    public required int InactiveCount { get; init; }

    public required int SoftDeletedCount { get; init; }

    public required int PermanentlyDeletedCount { get; init; }

    public required int WithImageCount { get; init; }

    public required int NewThisWeekCount { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public double ActiveRate => TotalCount == 0 ? 0 : (double)ActiveCount / TotalCount * 100;
}
