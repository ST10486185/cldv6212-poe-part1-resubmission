using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Functions.Models;

/*
Azure Table entity for the MenuItems table.
PartitionKey = Category (e.g. "Hot Drinks"), RowKey = SKU (e.g. "COF-001").

// Reference: Microsoft, n.d.-b
// https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.itableentity

 */

public class MenuItem : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public bool IsAvailable { get; set; }
}