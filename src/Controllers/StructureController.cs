using Microsoft.AspNetCore.Mvc;
using NanoBlog.Attributes;
using NanoBlog.Services.FileStorages;

namespace NanoBlog.Controllers;

[ApiController]
[Route("[controller]")]
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
        var fileNames = _fileStorage.GetFileNames();
        return Ok(fileNames);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName] string fileName,
        CancellationToken cancellationToken
    )
    {
        if (!_fileStorage.FileExists(fileName))
        {
            return NotFound(fileName);
        }

        var content = await _fileStorage.LoadContentAsync(fileName, cancellationToken);
        return Ok(content);
    }

    [HttpPut("{fileName}")]
    public async Task<IActionResult> UpdateFileContentAsync(
        [ValidFileName] string fileName,
        CancellationToken cancellationToken
    )
    {
        if (!_fileStorage.FileExists(fileName))
        {
            return NotFound(fileName);
        }

        await _fileStorage.WriteContentAsync(fileName, Request.Body, cancellationToken);
        _logger.LogInformation("Structure file {fileName} has been updated", fileName);

        return NoContent();
    }
}