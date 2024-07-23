using Documents.API.Response;

namespace Documents.API.Services;

public interface IBlobService 
{
    public Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);

    public Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}