namespace FutureTech.StudentManagement.Web.Options;

public sealed class BlobStorageOptions
{
    public const string SectionName = "Azure:BlobStorage";

    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "student-images";

    public int SasExpiryMinutes { get; set; } = 15;

    public int MaxUploadSizeMb { get; set; } = 5;

    public int ResizeWidth { get; set; } = 512;

    public int ResizeHeight { get; set; } = 512;
}
