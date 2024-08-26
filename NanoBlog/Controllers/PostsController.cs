namespace NanoBlog.Controllers;

[ApiController]
[Route("posts")]
public class PostsController(
    IStageDirectoryContainer stage,
    ILogger<PostsController> logger,
    IPostService postService
) : ControllerBase
{
    [HttpGet]
    public IActionResult GetFileNames()
    {
        var fileNames = stage.PostsDirectory
           .EnumerateFiles()
           .Select(f => f.Name)
           .OrderDescending();

        return Ok(fileNames);
    }

    [HttpGet("excerpts")]
    public async Task<IActionResult> GetExcerptsAsync(
        [FromQuery] int? length,
        CancellationToken cancellationToken
    )
    {
        return Ok(
            await postService.LoadExcerptsAsync(
                length ?? IPostService.EXCERPT_LENGTH_DEFAULT,
                cancellationToken
            )
        );
    }

    [HttpGet("template")]
    public async Task<IActionResult> GetPostTemplateAsync(CancellationToken cancellationToken)
    {
        await using var fileStream = stage.StructureDirectory
           .TryFindFileInfo(IConfiguration.STAGE_STRUCTURE_FILE_NAME_POST_TEMPLATE)?
           .OpenRead();

        if (fileStream is null)
        {
            return NotFound();
        }

        var content = await fileStream.LoadAsStringAsync(cancellationToken);
        return Content(content, "text/html");
    }

    [HttpPut("template")]
    public async Task<IActionResult> UpdatePostTemplateAsync(CancellationToken cancellationToken)
    {
        await using var fileStream = stage.StructureDirectory
           .TryFindFileInfo(IConfiguration.STAGE_STRUCTURE_FILE_NAME_POST_TEMPLATE)?
           .EnsureFileMode()
           .OpenWriteStream();

        if (fileStream is null)
        {
            return NotFound();
        }

        await Request.Body.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("Post template has been updated");
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(CancellationToken cancellationToken)
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.txt";

        await using var fileStream = stage.PostsDirectory.CreateFile(fileName);
        await Request.Body.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("Post {fileName} has been created", fileName);
        return CreatedAtAction("GetFileContent", new { fileName }, null);
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFileContentAsync(
        [ValidFileName.Text] string fileName,
        CancellationToken cancellationToken
    )
    {
        await using var fileStream = stage.PostsDirectory
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
        await using var fileStream = stage.PostsDirectory
           .TryFindFileInfo(fileName)?
           .EnsureFileMode()
           .OpenWriteStream();

        if (fileStream is null)
        {
            return NotFound();
        }

        await Request.Body.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("Post {fileName} has been updated", fileName);
        return NoContent();
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFileAsync([ValidFileName.Text] string fileName)
    {
        if (!stage.PostsDirectory.HasFile(fileName))
        {
            return NotFound();
        }

        stage.PostsDirectory.DeleteFile(fileName);

        logger.LogInformation("Post {fileName} has been deleted", fileName);
        return NoContent();
    }
}