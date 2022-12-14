using Microsoft.AspNetCore.Mvc;
using NanoBlog.Attributes;
using NanoBlog.Services.FileStorages.Posts;

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
        await using var fileStream = _fileStorage.Create();
        await _fileStorage.WriteContentAsync(fileStream, Request.Body, cancellationToken);

        _logger.LogInformation("Post {fileName} has been created", fileStream.Name);

        return NoContent();
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = _fileStorage.TryOpenReadStream(fileName);
        if (fileStream is null)
        {
            return NotFound(fileName);
        }

        var content = await _fileStorage.LoadContentAsync(fileStream, cancellationToken);

        return Ok(content);
    }

    [HttpPut("{fileName}")]
    public async Task<IActionResult> UpdateFileContentAsync(
        [ValidFileName] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = _fileStorage.TryOpenWriteStream(fileName);
        if (fileStream is null)
        {
            return NotFound(fileName);
        }

        await _fileStorage.WriteContentAsync(fileStream, Request.Body, cancellationToken);
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