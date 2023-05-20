namespace NanoBlog.Controllers;

[ApiController]
[Route("assets")]
public class AssetsController : ControllerBase
{
    private readonly IStageDirectoryContainer _stage;
    private readonly IMimeTypeProvider _mimeTypeProvider;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(
        IStageDirectoryContainer stage,
        IMimeTypeProvider mimeTypeProvider,
        ILogger<AssetsController> logger
    )
    {
        _stage = stage;
        _mimeTypeProvider = mimeTypeProvider;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetFileNames()
    {
        var fileNames = _stage.AssetsDirectory
           .EnumerateFiles()
           .Select(f => f.Name)
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

        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.{mimeType.GetExtension()}";

        await using var fileStream = _stage.AssetsDirectory.CreateFile(fileName);
        await content.CopyToAsync(fileStream, cancellationToken);

        _logger.LogInformation("Asset {fileName} has been created", fileName);
        return CreatedAtAction("GetFileContent", new { fileName }, null);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName.Asset] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = _stage.AssetsDirectory
           .TryFindFileInfo(fileName)?
           .OpenRead();

        if (fileStream is null)
        {
            return NotFound();
        }

        var mimeType = await _mimeTypeProvider.ProvideMimeTypeAsync(fileStream.Name, fileStream, cancellationToken)
            ?? throw new Exception("Could not determine MIME type of stored file");

        var content = await fileStream.LoadAsBytesAsync(cancellationToken);

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

        await using (var fileReadStream = _stage.AssetsDirectory.TryFindFileInfo(fileName)?.OpenRead())
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

        await using var fileWriteStream = _stage.AssetsDirectory
               .TryFindFileInfo(fileName)?
               .EnsureFileMode()
               .OpenWrite()
            ?? throw new FileNotFoundException($"Could not find file {fileName}");

        await content.CopyToAsync(fileWriteStream, cancellationToken);

        _logger.LogInformation("Asset {fileName} has been updated", fileName);
        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName.Asset] string fileName)
    {
        if (!_stage.AssetsDirectory.HasFile(fileName))
        {
            return NotFound();
        }

        _stage.AssetsDirectory.DeleteFile(fileName);

        _logger.LogInformation("Asset {fileName} has been deleted", fileName);
        return NoContent();
    }
}