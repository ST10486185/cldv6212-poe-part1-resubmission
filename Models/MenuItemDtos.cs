namespace CoffeeNChill.Functions.Models;

/// <summary>Request body for POST /api/menu.</summary>
public class CreateMenuItemDto
{
    public string Category { get; set; } = default!;
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public double Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}

/// <summary>Request body for PUT /api/menu/{category}/{id}. Both fields optional.</summary>
public class UpdateMenuItemDto
{
    public double? Price { get; set; }
    public bool? IsAvailable { get; set; }
}