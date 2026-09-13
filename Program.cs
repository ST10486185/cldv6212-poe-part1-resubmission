using Azure.Data.Tables;
using Azure.Storage.Files.Shares;
using CoffeeNChill.Functions.Services;
using CoffeeNChill_Part1.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Create the Azure Functions host
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Connection string for the local Azurite storage emulator
        string storageConnectionString =
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? "UseDevelopmentStorage=true";

        // Connection string for the Azure File Share
        string fileStorageConnectionString =
        Environment.GetEnvironmentVariable("AzureFilesConnectionString") 
        ?? throw new InvalidOperationException(
            "AzureFilesConnectionString is missing.");

        // Connect to Azure Table Storage
        services.AddSingleton(
            new TableServiceClient(storageConnectionString));

        // Connect to the Azurite Azure File Share
        services.AddSingleton(
            new ShareServiceClient(fileStorageConnectionString));

        // Register the menu service
        services.AddSingleton<IMenuTableService, MenuTableService>();

        // Register the document service
        services.AddSingleton<IStaffDocumentService, StaffDocumentService>();
    })
    .Build();

// Start the Azure Functions application
host.Run();