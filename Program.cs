using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using CoffeeNChill.Functions.Services;
using CoffeeNChill_Part1.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// falls back to Azurite's well-known connection string if nothing is set locally
var storageConnectionString =
    Environment.GetEnvironmentVariable("AzureWebJobsStorage")
    ?? "UseDevelopmentStorage=true";

builder.Services.AddSingleton(new TableServiceClient(storageConnectionString));
builder.Services.AddSingleton(new BlobServiceClient(storageConnectionString));

builder.Services.AddSingleton<IMenuTableService, MenuTableService>();
builder.Services.AddSingleton<IStaffDocumentService, StaffDocumentService>();

builder.Build().Run();
