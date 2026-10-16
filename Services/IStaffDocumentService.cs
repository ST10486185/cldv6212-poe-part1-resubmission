using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services;

// Reference: Microsoft, n.d.-c
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/interfaces

// wraps all the staff-docs blob storage calls so the functions never touch the SDK directly
public interface IStaffDocumentService
{
    Task<DocumentMetadata> UploadAsync(string fileName, Stream content, string contentType);
    Task<List<DocumentMetadata>> ListAsync();
    Task<(Stream Content, string ContentType)?> DownloadAsync(string fileName);
}
