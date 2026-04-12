namespace FutureTech.StudentManagement.Web.Options;

public sealed class CosmosOptions
{
    public const string SectionName = "Azure:Cosmos";

    public string Endpoint { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = "futuretech-students";

    public string ContainerName { get; set; } = "students";
}
