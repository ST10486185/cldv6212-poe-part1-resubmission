namespace CoffeeNChill.Functions.Models;

// Metadata returned for staff-docs blob operations.
public class DocumentMetadata
{
    public string FileName { get; set; } = default!;
    public long SizeInBytes { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string ContentType { get; set; } = default!;
}