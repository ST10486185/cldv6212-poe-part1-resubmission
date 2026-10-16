using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Functions.Models;

namespace CoffeeNChill_Part1.Services;

// Reference: Microsoft, n.d.-d
// https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableclient

public class MenuTableService : IMenuTableService
{
    private const string TableName = "MenuItems";
    private readonly TableClient _tableClient;

    // Constructor that initializes the TableClient and creates the table if it doesn't exist
    public MenuTableService(TableServiceClient tableServiceClient)
    {
        tableServiceClient.CreateTableIfNotExists(TableName);
        _tableClient = tableServiceClient.GetTableClient(TableName);
    }

    // Create a new menu item in the table
    public async Task<MenuItem> CreateAsync(MenuItem item)
    {
        await _tableClient.AddEntityAsync(item);
        return item;
    }

    // Retrieve all menu items from the table
    public async Task<List<MenuItem>> GetAllAsync()
    {
        var items = new List<MenuItem>();
        await foreach (var item in _tableClient.QueryAsync<MenuItem>())
        {
            items.Add(item);
        }
        return items;
    }

    // Retrieve menu items by category (PartitionKey) from the table
    public async Task<List<MenuItem>> GetByCategoryAsync(string category)
    {
        var items = new List<MenuItem>();
        await foreach (var item in _tableClient.QueryAsync<MenuItem>(x => x.PartitionKey == category))
        {
            items.Add(item);
        }
        return items;
    }

    // Retrieve a single menu item by category and id (PartitionKey and RowKey) from the table
    public async Task<MenuItem?> GetSingleAsync(string category, string id)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<MenuItem>(category, id);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    // Update an existing menu item in the table
    public async Task<MenuItem?> UpdateAsync(string category, string id, UpdateMenuItemDto dto)
    {
        var existing = await GetSingleAsync(category, id) 
            ?? throw new KeyNotFoundException($"Menu Item '{id}' in category '{category}' not found.");

        if (dto.Price.HasValue) existing.Price = dto.Price.Value;
        if (dto.IsAvailable.HasValue) existing.IsAvailable = dto.IsAvailable.Value;

        await _tableClient.UpdateEntityAsync(existing, existing.ETag, TableUpdateMode.Replace);
        return existing;
    }

    // Delete a menu item from the table
    public async Task DeleteAsync(string category, string id)
    {
        await _tableClient.DeleteEntityAsync(category, id);
    }
}
