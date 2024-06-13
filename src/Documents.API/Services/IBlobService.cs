using Documents.API.Response;

namespace Documents.API.Services;

public interface IBlobService 
{
    public Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);

    public Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}