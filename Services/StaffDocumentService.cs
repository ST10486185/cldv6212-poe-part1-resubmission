using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services;

public class StaffDocumentService : IStaffDocumentService
{
    private const string ShareName = "staff-docs";

    private readonly ShareDirectoryClient _directoryClient;

    public StaffDocumentService(ShareServiceClient shareServiceClient)
    {
        var shareClient = shareServiceClient.GetShareClient(ShareName);

        shareClient.CreateIfNotExists();

        _directoryClient = shareClient.GetRootDirectoryClient();
    }

    public async Task<DocumentMetadata> UploadAsync(
        string fileName,
        Stream content,
        string contentType)
    {
        // Copy the uploaded file into memory
        // so we know its size before creating the Azure File.
        using var memoryStream = new MemoryStream();

        await content.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        var fileClient =
            _directoryClient.GetFileClient(fileName);

        // Create the file with the correct size.
        await fileClient.CreateAsync(
            memoryStream.Length,
            new ShareFileCreateOptions
            {
                HttpHeaders = new ShareFileHttpHeaders
                {
                    ContentType = contentType
                }
            });

        // Upload the file contents.
        await fileClient.UploadRangeAsync(
            new HttpRange(0, memoryStream.Length),
            memoryStream);

        var properties =
            await fileClient.GetPropertiesAsync();

        return new DocumentMetadata
        {
            FileName = fileName,
            SizeInBytes = properties.Value.ContentLength,
            LastModified = properties.Value.LastModified,
            ContentType = contentType
        };
    }

    public async Task<List<DocumentMetadata>> ListAsync()
    {
        var documents = new List<DocumentMetadata>();

        await foreach (
            var item in _directoryClient.GetFilesAndDirectoriesAsync())
        {
            if (!item.IsDirectory)
            {
                var fileClient =
                    _directoryClient.GetFileClient(item.Name);

                var properties =
                    await fileClient.GetPropertiesAsync();

                documents.Add(new DocumentMetadata
                {
                    FileName = item.Name,
                    SizeInBytes = properties.Value.ContentLength,
                    LastModified = properties.Value.LastModified,
                    ContentType =
                        properties.Value.ContentType
                        ?? "application/octet-stream"
                });
            }
        }

        return documents;
    }

    public async Task<(Stream Content, string ContentType)?> DownloadAsync(
        string fileName)
    {
        var fileClient =
            _directoryClient.GetFileClient(fileName);

        if (!await fileClient.ExistsAsync())
        {
            return null;
        }

        var download =
            await fileClient.DownloadAsync();

        var properties =
            await fileClient.GetPropertiesAsync();

        return (
            download.Value.Content,
            properties.Value.ContentType
            ?? "application/octet-stream"
        );
    }
}