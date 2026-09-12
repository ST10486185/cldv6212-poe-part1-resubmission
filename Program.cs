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

        // Full Azurite connection string for Azure Files
        string fileStorageConnectionString =
            "DefaultEndpointsProtocol=http;" +
            "AccountName=devstoreaccount1;" +
            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
            "FileEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

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