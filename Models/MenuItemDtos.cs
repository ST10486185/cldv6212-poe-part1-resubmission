namespace CoffeeNChill.Functions.Models;

// Request body for POST /api/menu.
public class CreateMenuItemDto
{
    public string Category { get; set; } = default!;
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public double Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}

// Request body for PUT /api/menu/{category}/{id}. Both fields optional.
public class UpdateMenuItemDto
{
    public double? Price { get; set; }
    public bool? IsAvailable { get; set; }
}