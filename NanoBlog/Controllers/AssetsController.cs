namespace NanoBlog.Controllers;

[ApiController]
[Route("assets")]
public class AssetsController(
    IStageDirectoryContainer stage,
    IMimeTypeProvider mimeTypeProvider,
    ILogger<AssetsController> logger
) : ControllerBase
{
    [HttpGet]
    public IActionResult GetFileNames()
    {
        var fileNames = stage.AssetsDirectory
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

        if (await mimeTypeProvider.ProvideMimeTypeAsync(file, content, cancellationToken) is not { } mimeType)
        {
            return BadRequest();
        }

        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.{mimeType.GetExtension()}";

        await using var fileStream = stage.AssetsDirectory.CreateFile(fileName);
        await content.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("Asset {fileName} has been created", fileName);
        return CreatedAtAction("GetFileContent", new { fileName }, null);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName.Asset] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = stage.AssetsDirectory
           .TryFindFileInfo(fileName)?
           .OpenRead();

        if (fileStream is null)
        {
            return NotFound();
        }

        var mimeType = await mimeTypeProvider.ProvideMimeTypeAsync(fileStream.Name, fileStream, cancellationToken)
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
        var fileInfo = stage.AssetsDirectory.TryFindFileInfo(fileName);
        if (fileInfo is null)
        {
            return NotFound();
        }

        var content = new MemoryStream();
        await using (var formFileStream = file.OpenReadStream())
        {
            await formFileStream.CopyToAsync(content, cancellationToken);
        }

        content.Position = 0;

        if (await mimeTypeProvider.ProvideMimeTypeAsync(file, content, cancellationToken) is not { } uploadMimeType)
        {
            return BadRequest();
        }

        await using (var fileReadStream = fileInfo.OpenRead())
        {
            var storedFileMimeType = await mimeTypeProvider.ProvideMimeTypeAsync(
                fileReadStream.Name,
                fileReadStream,
                cancellationToken
            ) ?? throw new Exception("Could not determine MIME type of stored file");

            if (storedFileMimeType != uploadMimeType)
            {
                return BadRequest();
            }
        }

        await using var fileWriteStream = fileInfo
           .EnsureFileMode()
           .OpenWriteStream();

        await content.CopyToAsync(fileWriteStream, cancellationToken);

        logger.LogInformation("Asset {fileName} has been updated", fileName);
        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName.Asset] string fileName)
    {
        if (!stage.AssetsDirectory.HasFile(fileName))
        {
            return NotFound();
        }

        stage.AssetsDirectory.DeleteFile(fileName);

        logger.LogInformation("Asset {fileName} has been deleted", fileName);
        return NoContent();
    }
}