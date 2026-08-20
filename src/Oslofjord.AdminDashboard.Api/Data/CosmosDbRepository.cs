using Microsoft.Azure.Cosmos;

namespace Oslofjord.AdminDashboard.Api.Data;

public interface ICosmosDbRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id, string? partitionKey = null);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> QueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<T> CreateAsync(T item, string? partitionKey = null);
    Task<T> UpdateAsync(string id, T item, string? partitionKey = null);
    Task DeleteAsync(string id, string? partitionKey = null);
}

public class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : class
{
    private readonly Container _container;
    
    public CosmosDbRepository(Container container)
    {
        _container = container;
    }
    
    public async Task<T?> GetByIdAsync(string id, string? partitionKey = null)
    {
        try
        {
            var response = partitionKey != null
                ? await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey))
                : await _container.ReadItemAsync<T>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = new QueryDefinition("SELECT * FROM c");
        return await ExecuteQueryAsync(query);
    }
    
    public async Task<IEnumerable<T>> QueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        var queryDefinition = new QueryDefinition(query);
        
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                queryDefinition = queryDefinition.WithParameter(param.Key, param.Value);
            }
        }
        
        return await ExecuteQueryAsync(queryDefinition);
    }
    
    public async Task<T> CreateAsync(T item, string? partitionKey = null)
    {
        var response = await _container.CreateItemAsync(item, 
            partitionKey != null ? new PartitionKey(partitionKey) : PartitionKey.None);
        return response.Resource;
    }
    
    public async Task<T> UpdateAsync(string id, T item, string? partitionKey = null)
    {
        var response = partitionKey != null
            ? await _container.ReplaceItemAsync(item, id, new PartitionKey(partitionKey))
            : await _container.ReplaceItemAsync(item, id, new PartitionKey(id));
        return response.Resource;
    }
    
    public async Task DeleteAsync(string id, string? partitionKey = null)
    {
        if (partitionKey != null)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }
        else
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }
    }
    
    private async Task<IEnumerable<T>> ExecuteQueryAsync(QueryDefinition queryDefinition)
    {
        var results = new List<T>();
        var iterator = _container.GetItemQueryIterator<T>(queryDefinition);
        
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        
        return results;
    }
}
