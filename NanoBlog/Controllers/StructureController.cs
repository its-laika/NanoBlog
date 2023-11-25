namespace NanoBlog.Controllers;

[ApiController]
[Route("structure")]
public class StructureController(
    IStageDirectoryContainer stage,
    ILogger<StructureController> logger
) : ControllerBase
{
    [HttpGet]
    public IActionResult GetFileNames()
    {
        var fileNames = stage.StructureDirectory
           .EnumerateFiles()
           .Select(f => f.Name)
           .OrderDescending();

        return Ok(fileNames);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = stage.StructureDirectory
           .TryFindFileInfo(fileName)?
           .OpenRead();

        if (fileStream is null)
        {
            return NotFound();
        }

        var content = await fileStream.LoadAsStringAsync(cancellationToken);
        return Content(content, "text/html");
    }

    [HttpPut("{fileName}")]
    public async Task<IActionResult> UpdateFileContentAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = stage.StructureDirectory
           .TryFindFileInfo(fileName)?
           .EnsureFileMode()
           .OpenWriteStream();

        if (fileStream is null)
        {
            return NotFound();
        }

        await Request.Body.CopyToAsync(fileStream, cancellationToken);
        logger.LogInformation("Structure file {fileName} has been updated", fileName);

        return NoContent();
    }
}