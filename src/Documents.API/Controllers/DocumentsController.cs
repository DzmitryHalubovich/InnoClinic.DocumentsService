using Azure;
using Documents.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Documents.API.Controllers;

[Route("api/documents")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly IBlobService _blobService;

    public DocumentsController(IBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument(IFormFile file, CancellationToken cancellationToken)
    {
        using Stream stream = file.OpenReadStream();

        var fileId = await _blobService.UploadAsync(stream, file.ContentType, cancellationToken);

        return Ok(fileId);
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> DownloadDocument(Guid fileId, CancellationToken cancellationToken)
    {
        try
        {
            var fileResponse = await _blobService.DownloadAsync(fileId, cancellationToken);

            return File(fileResponse.Stream, fileResponse.ContentType);
        }
        catch (RequestFailedException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteDocument(Guid fileId, CancellationToken cancellationToken)
    {
        await _blobService.DeleteAsync(fileId, cancellationToken);
        
        return NoContent();
    }
}
