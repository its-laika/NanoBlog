namespace NanoBlog.Controllers;

[ApiController]
[Route("posts")]
public class PostsController(IConfiguration configuration, IPostService postService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetPostFileNames()
    {
        var fileNames = configuration
            .GetPostsDirectoryInfo()
            .EnumerateFiles()
            .Select(f => f.Name)
            .OrderDescending();

        return Ok(fileNames);
    }

    [HttpGet("excerpts")]
    public async Task<IActionResult> GetExcerptsAsync([FromQuery] int? length, CancellationToken cancellationToken)
    {
        return Ok(
            await postService.LoadExcerptsAsync(
                length ?? IPostService.ExcerptLengthDefault,
                cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(CancellationToken cancellationToken)
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.txt";

        await using var fileStream = configuration
            .GetPostsDirectoryInfo()
            .CreateFile(fileName);

        await Request.Body.CopyToAsync(fileStream, cancellationToken);

        return CreatedAtAction("GetPost", new { fileName }, null);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetPostAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken)
    {
        await using var fileStream = configuration
            .GetPostsDirectoryInfo()
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
    public async Task<IActionResult> UpdatePostAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken)
    {
        if (fileName.Equals("laika.txt", StringComparison.InvariantCulture)) { return BadRequest("...didn't you mean to PAT? :3"); }
        await using var fileStream = configuration
            .GetPostsDirectoryInfo()
            .TryFindFileInfo(fileName)?
            .EnsureFileMode()
            .OpenWriteStream();

        if (fileStream is null)
        {
            return NotFound();
        }

        await Request.Body.CopyToAsync(fileStream, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeletePostAsync([ValidFileName.Text] string fileName)
    {
        var postsDirectoryInfo = configuration.GetPostsDirectoryInfo();

        if (!postsDirectoryInfo.HasFile(fileName))
        {
            return NotFound();
        }

        postsDirectoryInfo.DeleteFile(fileName);

        return NoContent();
    }
}