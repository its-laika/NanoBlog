using Microsoft.AspNetCore.Mvc;
using NanoBlog.Attributes;
using NanoBlog.Services.FileStorages;

namespace NanoBlog.Controllers;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    private readonly IPostsFileStorage _fileStorage;
    private readonly ILogger<PostsController> _logger;

    public PostsController(
        IPostsFileStorage fileStorage,
        ILogger<PostsController> logger
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

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(CancellationToken cancellationToken)
    {
        var fileName = await _fileStorage.CreatePostFileAsync(Request.Body, cancellationToken);
        _logger.LogInformation("Post {fileName} has been created", fileName);

        return Created($"/posts/{fileName}", null);
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
        _logger.LogInformation("Post {fileName} has been updated", fileName);

        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName] string fileName)
    {
        if (!_fileStorage.FileExists(fileName))
        {
            return NotFound(fileName);
        }

        _fileStorage.Delete(fileName);
        _logger.LogInformation("Post {fileName} has been deleted", fileName);

        return NoContent();
    }
}