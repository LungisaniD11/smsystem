using FutureTech.StudentManagement.Web.Domain;
using FutureTech.StudentManagement.Web.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FutureTech.StudentManagement.Web.Services;

public sealed class CosmosStudentRepository : IStudentRepository
{
    private readonly Container _container;

    public CosmosStudentRepository(IOptions<CosmosOptions> options)
    {
        var config = options.Value;
        var clientOptions = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        var client = new CosmosClient(config.Endpoint, config.Key, clientOptions);
        var database = client.CreateDatabaseIfNotExistsAsync(config.DatabaseName).GetAwaiter().GetResult();
        _container = database.Database.CreateContainerIfNotExistsAsync(config.ContainerName, "/id").GetAwaiter().GetResult().Container;
    }

    public async Task<StudentRecord?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<StudentRecord>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<PagedResult<StudentRecord>> SearchAsync(string? query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var normalizedQuery = query?.Trim() ?? string.Empty;
        var whereClause = string.IsNullOrWhiteSpace(normalizedQuery)
            ? string.Empty
            : "AND (CONTAINS(c.firstName, @query, true) OR CONTAINS(c.lastName, @query, true) OR CONTAINS(c.id, @query, true))";

        var countSql = $"SELECT VALUE COUNT(1) FROM c WHERE 1 = 1 {whereClause}";
        var countDefinition = new QueryDefinition(countSql);
        if (!string.IsNullOrWhiteSpace(normalizedQuery))
        {
            countDefinition.WithParameter("@query", normalizedQuery);
        }

        var countIterator = _container.GetItemQueryIterator<int>(countDefinition);
        var totalCount = 0;
        while (countIterator.HasMoreResults)
        {
            var countPage = await countIterator.ReadNextAsync(cancellationToken);
            totalCount += countPage.Resource.FirstOrDefault();
        }

        var offset = (pageNumber - 1) * pageSize;
        var sql = $"SELECT * FROM c WHERE 1 = 1 {whereClause} ORDER BY c.updatedAtUtc DESC OFFSET @offset LIMIT @limit";
        var queryDefinition = new QueryDefinition(sql)
            .WithParameter("@offset", offset)
            .WithParameter("@limit", pageSize);

        if (!string.IsNullOrWhiteSpace(normalizedQuery))
        {
            queryDefinition.WithParameter("@query", normalizedQuery);
        }

        var iterator = _container.GetItemQueryIterator<StudentRecord>(queryDefinition);
        var results = new List<StudentRecord>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response.Resource);
        }

        return new PagedResult<StudentRecord>
        {
            Items = results,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task UpsertAsync(StudentRecord student, CancellationToken cancellationToken = default)
    {
        await _container.UpsertItemAsync(student, new PartitionKey(student.Id), cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _container.DeleteItemAsync<StudentRecord>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}
