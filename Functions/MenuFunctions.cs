using System.Net;
using System.Text.Json;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Services;
using CoffeeNChill_Part1.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CoffeeNChill.Functions;

// Reference: Microsoft, n.d.-a
// https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger

public class MenuFunctions
{
    // This connects the function to the menu table service
    private readonly IMenuTableService _menuService;

    // Gets the menu service when the function starts
    public MenuFunctions(IMenuTableService menuService)
    {
        _menuService = menuService;
    }

    // POST /api/menu
    // Creates a new menu item
    [Function("CreateMenuItem")]
    public async Task<HttpResponseData> CreateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")] HttpRequestData req)
    {
        try
        {
            // Read the JSON sent in the request
            var menuItem = await JsonSerializer.DeserializeAsync<CreateMenuItemDto>(req.Body);

            // Check if the request contains valid data
            if (menuItem == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid menu item data.");
                return badResponse;
            }

            // Check that the required fields were entered
            if (string.IsNullOrWhiteSpace(menuItem.Category) ||
                string.IsNullOrWhiteSpace(menuItem.Sku) ||
                string.IsNullOrWhiteSpace(menuItem.Name))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Category, SKU and Name are required.");
                return badResponse;
            }

            // Make sure the price is not negative
            if (menuItem.Price < 0)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Price cannot be negative.");
                return badResponse;
            }

            // Create a menu item using the information from the request
            var item = new MenuItem
            {
                // Category is stored as the PartitionKey
                PartitionKey = menuItem.Category,

                // SKU is stored as the RowKey
                RowKey = menuItem.Sku,

                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable
            };

            // Save the menu item to Azure Table Storage
            var createdItem = await _menuService.CreateAsync(item);

            // Return a 201 Created response with the new menu item
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdItem);

            return response;
        }
        catch (InvalidOperationException)
        {
            // This is returned if the SKU already exists
            var response = req.CreateResponse(HttpStatusCode.Conflict);
            await response.WriteStringAsync("A menu item with this SKU already exists.");
            return response;
        }
        catch (JsonException)
        {
            // This is returned if the request contains invalid JSON
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync("Invalid JSON.");
            return response;
        }
    }

    // GET /api/menu
    // Gets all menu items from the MenuItems table
    [Function("GetAllMenuItems")]
    public async Task<HttpResponseData> GetAllMenuItems(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")] HttpRequestData req)
    {
        // Get all menu items from the service
        var items = await _menuService.GetAllAsync();

        // Return the menu items with a 200 OK response
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(items);

        return response;
    }

    // GET /api/menu/category/{category}
    // Gets menu items that belong to a specific category
    [Function("GetMenuItemsByCategory")]
    public async Task<HttpResponseData> GetMenuItemsByCategory(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu/category/{category}")] HttpRequestData req,
        string category)
    {
        // Check that a category was provided
        if (string.IsNullOrWhiteSpace(category))
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync("Category is required.");

            return response;
        }

        // Get menu items for the selected category
        var items = await _menuService.GetByCategoryAsync(category);

        // Return the filtered menu items
        var responseOk = req.CreateResponse(HttpStatusCode.OK);
        await responseOk.WriteAsJsonAsync(items);

        return responseOk;
    }

    // PUT /api/menu/{category}/{id}
    // Updates the price or availability of a menu item
    [Function("UpdateMenuItem")]
    public async Task<HttpResponseData> UpdateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "menu/{category}/{id}")] HttpRequestData req,
        string category,
        string id)
    {
        // Check that the category and ID were provided
        if (string.IsNullOrWhiteSpace(category) ||
            string.IsNullOrWhiteSpace(id))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Category and ID are required.");

            return badResponse;
        }

        UpdateMenuItemDto? updateData;

        try
        {
            // Read the update information from the request
            updateData = await JsonSerializer.DeserializeAsync<UpdateMenuItemDto>(req.Body);
        }
        catch (JsonException)
        {
            // Return an error if the JSON is invalid
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid JSON.");

            return badResponse;
        }

        // Check that update data was provided
        if (updateData == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Update data is required.");

            return badResponse;
        }

        // Check that the price is not negative
        if (updateData.Price.HasValue && updateData.Price.Value < 0)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Price cannot be negative.");

            return badResponse;
        }

        try
        {
            // Update the menu item using the service
            var updatedItem = await _menuService.UpdateAsync(
                category,
                id,
                updateData);

            // Return the updated menu item
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedItem);

            return response;
        }
        catch (KeyNotFoundException)
        {
            // Return 404 if the menu item does not exist
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteStringAsync("Menu item not found.");

            return response;
        }
    }

    // DELETE /api/menu/{category}/{id}
    // Deletes a menu item
    [Function("DeleteMenuItem")]
    public async Task<HttpResponseData> DeleteMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "menu/{category}/{id}")] HttpRequestData req,
        string category,
        string id)
    {
        // Check that the category and ID were provided
        if (string.IsNullOrWhiteSpace(category) ||
            string.IsNullOrWhiteSpace(id))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Category and ID are required.");

            return badResponse;
        }

        try
        {
            // Delete the menu item from Azure Table Storage
            await _menuService.DeleteAsync(category, id);

            // Return a successful response
            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            // Return 404 if the menu item does not exist
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteStringAsync("Menu item not found.");

            return response;
        }
    }
}