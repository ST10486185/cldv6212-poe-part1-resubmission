using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using CoffeeNChill.Functions.Models;
using CoffeeNChill.Functions.Services;

namespace CoffeeNChill.Functions;

// Reference: Microsoft, n.d.-a
// https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger

public class DocumentFunctions
{
    // This connects the function to the staff document service
    private readonly IStaffDocumentService _documentService;

    // Gets the document service when the function starts
    public DocumentFunctions(IStaffDocumentService documentService)
    {
        _documentService = documentService;
    }

    // POST /api/documents/upload
    // Uploads a file to the staff-docs Azure File Share
    [Function("UploadDocument")]
    public async Task<HttpResponseData> UploadDocument(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "documents/upload")] HttpRequestData req)
    {
        // Get the Content-Type from the request
        if (!req.Headers.TryGetValues("Content-Type", out var contentTypes))
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync("Content-Type is required.");
            return response;
        }

        var contentType = contentTypes.FirstOrDefault();

        // Make sure the request uses multipart/form-data
        if (contentType == null ||
            !contentType.StartsWith("multipart/form-data"))
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync(
                "The request must use multipart/form-data.");

            return response;
        }

        // Find the boundary from the Content-Type
        var boundaryText = contentType
            .Split(';')
            .FirstOrDefault(x => x.Trim().StartsWith("boundary="));

        if (boundaryText == null)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync("Multipart boundary is missing.");
            return response;
        }

        // Remove "boundary=" from the value
        var boundary = boundaryText
            .Trim()
            .Substring("boundary=".Length)
            .Trim('"');

        // Read the multipart form data
        var reader = new MultipartReader(boundary, req.Body);

        MultipartSection? section;

        while ((section = await reader.ReadNextSectionAsync()) != null)
        {
            // Get information about the uploaded file
            var contentDisposition =
                section.Headers["Content-Disposition"].FirstOrDefault();

            if (contentDisposition == null)
            {
                continue;
            }

            // Read the Content-Disposition information
            var disposition =
                System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(
                    contentDisposition);

            // Check if this section contains a file
            if (string.IsNullOrEmpty(disposition.FileName))
            {
                continue;
            }

            // Get the file name
            var fileName = Path.GetFileName(disposition.FileName);

            // Get the file extension
            string fileExtension =
                Path.GetExtension(fileName).ToLowerInvariant();

            // Only allow PDF, DOCX and TXT files
            if (fileExtension != ".pdf" &&
                fileExtension != ".docx" &&
                fileExtension != ".txt")
            {
                var response = req.CreateResponse(
                    HttpStatusCode.BadRequest);

                await response.WriteStringAsync(
                    "Only PDF, DOCX and TXT files are allowed.");

                return response;
            }

            // Get the file content type
            var fileContentType =
                section.Headers["Content-Type"].FirstOrDefault()
                ?? "application/octet-stream";

            // Upload the file to the Azure File Share
            var document = await _documentService.UploadAsync(
                fileName,
                section.Body,
                fileContentType);

            // Return information about the uploaded file
            var successResponse = req.CreateResponse(
                HttpStatusCode.Created);

            await successResponse.WriteAsJsonAsync(document);

            return successResponse;
        }

        // Return an error if no file was found
        var badFileResponse = req.CreateResponse(
            HttpStatusCode.BadRequest);

        await badFileResponse.WriteStringAsync(
            "No file was uploaded.");

        return badFileResponse;
    }

    // GET /api/documents
    // Lists the documents stored in the staff-docs File Share
    [Function("GetDocuments")]
    public async Task<HttpResponseData> GetDocuments(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents")] HttpRequestData req)
    {
        // Get the documents from the service
        var documents = await _documentService.ListAsync();

        // Return the documents with a 200 OK response
        var response = req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(documents);

        return response;
    }

    // GET /api/documents/download/{fileName}
    // Downloads a file from the staff-docs File Share
    [Function("DownloadDocument")]
    public async Task<HttpResponseData> DownloadDocument(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents/download/{fileName}")] HttpRequestData req,
        string fileName)
    {
        // Check that a file name was provided
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync("File name is required.");

            return response;
        }

        // Get the file from the document service
        var document = await _documentService.DownloadAsync(fileName);

        // Check if the file exists
        if (document == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteStringAsync("File not found.");

            return response;
        }

        // Create a successful response
        var successResponse = req.CreateResponse(HttpStatusCode.OK);

        // Set the content type of the downloaded file
        successResponse.Headers.Add(
            "Content-Type",
            document.Value.ContentType);

        // Copy the file into the response
        await document.Value.Content.CopyToAsync(
            successResponse.Body);

        return successResponse;
    }
}