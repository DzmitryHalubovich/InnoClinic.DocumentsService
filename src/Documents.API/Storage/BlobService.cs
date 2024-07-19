using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Documents.API.Response;
using Documents.API.Services;

namespace Documents.API.Storage;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public const string ContainerName  = "documents";

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(fileId.ToString());

        try
        {
            var response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);

            return new FileResponse(response.Value.Content.ToStream(), response.Value.Details.ContentType);
        }
        catch (RequestFailedException ex)
        {
            throw new RequestFailedException(ex.Status, ex.Message);
        }
    }
}
