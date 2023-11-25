namespace NanoBlog.Controllers;

[ApiController]
public class ExportController(
    IExportationService exportationService,
    IBlogGenerator blogGenerator,
    ILogger<ExportController> logger
) : ControllerBase
{
    [HttpPost("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        await exportationService.ExportAsync(cancellationToken);

        logger.LogInformation("Exported successfully");
        return NoContent();
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview(CancellationToken cancellationToken)
    {
        var preview = await blogGenerator.GeneratePreviewAsync(cancellationToken);

        return Content(preview, "text/html");
    }
}