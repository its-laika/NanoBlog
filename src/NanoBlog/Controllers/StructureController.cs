using Microsoft.AspNetCore.Mvc;
using NanoBlog.Attributes;
using NanoBlog.Services.FileStorages.Structure;

namespace NanoBlog.Controllers;

[ApiController]
[Route("structure")]
public class StructureController : ControllerBase
{
    private readonly IStructureFileStorage _fileStorage;
    private readonly ILogger<StructureController> _logger;

    public StructureController(
        IStructureFileStorage fileStorage,
        ILogger<StructureController> logger
    )
    {
        _fileStorage = fileStorage;
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

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = _fileStorage.TryOpenReadStream(fileName);
        if (fileStream is null)
        {
            return NotFound();
        }

        var content = await _fileStorage.LoadContentAsync(fileStream, cancellationToken);
        return Content(content, "text/html");
    }

    [HttpPut("{fileName}")]
    public async Task<IActionResult> UpdateFileContentAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = _fileStorage.TryOpenWriteStream(fileName);
        if (fileStream is null)
        {
            return NotFound();
        }

        await Request.Body.CopyToAsync(fileStream, cancellationToken);
        _logger.LogInformation("Structure file {fileName} has been updated", fileName);

        return NoContent();
    }
}