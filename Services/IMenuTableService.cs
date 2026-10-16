using CoffeeNChill.Functions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeNChill_Part1.Services
{

    // Reference: Microsoft, n.d.-c
    // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces

    public interface IMenuTableService
    {
        // Create new menu item
        Task<MenuItem> CreateAsync(MenuItem item);
        // Get all menu items
        Task<List<MenuItem>> GetAllAsync();
        // Get menu items by category
        Task<List<MenuItem>> GetByCategoryAsync(string category);
        // Get single menu item by category and id
        Task<MenuItem?> GetSingleAsync(string category, string id);
        // Update existing menu item by category and id
        Task<MenuItem?> UpdateAsync(string category, string id, UpdateMenuItemDto dto);
        // Delete an item
        Task DeleteAsync(string category, string id);
    }
}
