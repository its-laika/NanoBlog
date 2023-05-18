namespace NanoBlog.Controllers;

[ApiController]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;
    private readonly IBlogGenerator _blogGenerator;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        IExportService exportService,
        IBlogGenerator blogGenerator,
        ILogger<ExportController> logger
    )
    {
        _exportService = exportService;
        _blogGenerator = blogGenerator;
        _logger = logger;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        await _exportService.ExportAsync(cancellationToken);

        _logger.LogInformation("Exported successfully");
        return NoContent();
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview(CancellationToken cancellationToken)
    {
        var generatedContent = await _blogGenerator.GeneratePreviewAsync(cancellationToken);
        return Content(generatedContent, "text/html");
    }
}