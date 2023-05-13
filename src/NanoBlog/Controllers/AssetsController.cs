using Microsoft.AspNetCore.Mvc;
using NanoBlog.Attributes;
using NanoBlog.Services.FileStorages.Assets;
using NanoBlog.Services.MimeTypes;

namespace NanoBlog.Controllers;

[ApiController]
[Route("assets")]
public class AssetsController : ControllerBase
{
    private readonly IAssetsFileStorage _fileStorage;
    private readonly IMimeTypeProvider _mimeTypeProvider;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(
        IAssetsFileStorage fileStorage,
        IMimeTypeProvider mimeTypeProvider,
        ILogger<AssetsController> logger
    )
    {
        _fileStorage = fileStorage;
        _mimeTypeProvider = mimeTypeProvider;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetFileNames()
    {
        var fileNames = _fileStorage
            .GetFileNames()
            .OrderDescending();

        return Ok(fileNames);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssetAsync([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        var content = new MemoryStream();
        await using (var formFileStream = file.OpenReadStream())
        {
            await formFileStream.CopyToAsync(content, cancellationToken);
        }

        content.Position = 0;

        if (await _mimeTypeProvider.ProvideMimeTypeAsync(file, content, cancellationToken) is not { } mimeType)
        {
            return BadRequest();
        }

        await using var fileStream = _fileStorage.CreateWriteStream(mimeType);
        await content.CopyToAsync(fileStream, cancellationToken);

        var fileName = Path.GetFileName(fileStream.Name);
        _logger.LogInformation("Asset {fileName} has been created", fileName);

        return CreatedAtAction("GetFileContent", new { fileName }, null);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName.Asset] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = _fileStorage.TryOpenReadStream(fileName);
        if (fileStream is null)
        {
            return NotFound();
        }

        var mimeType = await _mimeTypeProvider.ProvideMimeTypeAsync(fileStream.Name, fileStream, cancellationToken)
                       ?? throw new Exception("Could not determine MIME type of stored file");

        var content = await _fileStorage.LoadContentAsync(fileStream, cancellationToken);

        return new FileContentResult(content, mimeType.AsString());
    }

    [HttpPut("{fileName}")]
    public async Task<IActionResult> UpdateFileContentAsync(
        [ValidFileName.Asset] string fileName,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken
    )
    {
        var content = new MemoryStream();
        await using (var formFileStream = file.OpenReadStream())
        {
            await formFileStream.CopyToAsync(content, cancellationToken);
        }

        content.Position = 0;

        if (await _mimeTypeProvider.ProvideMimeTypeAsync(file, content, cancellationToken) is not { } uploadMimeType)
        {
            return BadRequest();
        }

        await using (var fileReadStream = _fileStorage.TryOpenReadStream(fileName))
        {
            if (fileReadStream is null)
            {
                return NotFound();
            }

            var storedFileMimeType = await _mimeTypeProvider.ProvideMimeTypeAsync(
                fileReadStream.Name,
                fileReadStream,
                cancellationToken
            ) ?? throw new Exception("Could not determine MIME type of stored file");

            if (storedFileMimeType != uploadMimeType)
            {
                return BadRequest();
            }
        }

        await using var fileWriteStream = _fileStorage.OpenWriteStream(fileName);
        await content.CopyToAsync(fileWriteStream, cancellationToken);

        _logger.LogInformation("Asset {fileName} has been updated", Path.GetFileName(fileWriteStream.Name));

        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName.Asset] string fileName)
    {
        if (!_fileStorage.FileExists(fileName))
        {
            return NotFound();
        }

        _fileStorage.Delete(fileName);
        _logger.LogInformation("Asset {fileName} has been deleted", fileName);

        return NoContent();
    }
}