using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CoffeeNChill.Functions.Models;

namespace CoffeeNChill.Functions.Services;

public class StaffDocumentService : IStaffDocumentService
{
    private const string ContainerName = "staff-docs";
    private readonly BlobContainerClient _containerClient;

    public StaffDocumentService(BlobServiceClient blobServiceClient)
    {
        _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<DocumentMetadata> UploadAsync(string fileName, Stream content, string contentType)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        var headers = new BlobHttpHeaders { ContentType = contentType };

        // streams straight into blob storage instead of loading the whole file into memory first
        await blobClient.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers });

        var props = await blobClient.GetPropertiesAsync();
        return new DocumentMetadata
        {
            FileName = fileName,
            SizeInBytes = props.Value.ContentLength,
            LastModified = props.Value.LastModified,
            ContentType = props.Value.ContentType
        };
    }

    public async Task<List<DocumentMetadata>> ListAsync()
    {
        var docs = new List<DocumentMetadata>();

        // grabs every blob in the container along with its metadata
        await foreach (var blob in _containerClient.GetBlobsAsync())
        {
            docs.Add(new DocumentMetadata
            {
                FileName = blob.Name,
                SizeInBytes = blob.Properties.ContentLength ?? 0,
                LastModified = blob.Properties.LastModified,
                ContentType = blob.Properties.ContentType ?? "application/octet-stream"
            });
        }

        return docs;
    }

    public async Task<(Stream Content, string ContentType)?> DownloadAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        // streamed back to the caller rather than buffered fully in memory
        var download = await blobClient.DownloadStreamingAsync();
        return (download.Value.Content, download.Value.Details.ContentType ?? "application/octet-stream");
    }
}
