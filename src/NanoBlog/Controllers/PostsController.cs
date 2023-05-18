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
        var fileNames = _fileStorage
            .GetFileInfos()
            .Select(f => f.Name)
            .OrderDescending();

        return Ok(fileNames);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(CancellationToken cancellationToken)
    {
        await using var fileStream = _fileStorage.CreateNewFileWriteStream();
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
            return NotFound();
        }

        var content = await _fileStorage.LoadContentAsStringAsync(fileStream, cancellationToken);
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
        _logger.LogInformation("Post {fileName} has been updated", fileName);

        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName.Text] string fileName)
    {
        if (!_fileStorage.FileExists(fileName))
        {
            return NotFound();
        }

        _fileStorage.Delete(fileName);
        _logger.LogInformation("Post {fileName} has been deleted", fileName);

        return NoContent();
    }
}