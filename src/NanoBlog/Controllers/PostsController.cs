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
        await using var fileStream = _fileStorage.CreateWriteStream();
        await Request.Body.CopyToAsync(fileStream, cancellationToken);

        var fileName = Path.GetFileName(fileStream.Name);
        _logger.LogInformation("Post {fileName} has been created", fileName);

        return CreatedAtAction("GetFileContent", new { fileName }, null);
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
            return NotFound(fileName);
        }

        var content = await _fileStorage.LoadContentAsync(fileStream, cancellationToken);

        return Ok(content);
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
            return NotFound(fileName);
        }

        await Request.Body.CopyToAsync(fileStream, cancellationToken);
        _logger.LogInformation("Post {fileName} has been updated", fileName);

        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName.Text] string fileName)
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