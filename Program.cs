using Azure.Data.Tables;
using Azure.Storage.Files.Shares;
using CoffeeNChill.Functions.Services;
using CoffeeNChill_Part1.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        string storageConnectionString =
            Environment.GetEnvironmentVariable("AzureWebJobsStorage")
            ?? "UseDevelopmentStorage=true";

        services.AddSingleton(
            new TableServiceClient(storageConnectionString));

        services.AddSingleton(
            new ShareServiceClient(storageConnectionString));

        services.AddSingleton<IMenuTableService, MenuTableService>();

        services.AddSingleton<IStaffDocumentService, StaffDocumentService>();
    })
    .Build();

host.Run();