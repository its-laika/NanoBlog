namespace NanoBlog.Controllers;

[ApiController]
public class ExportController : ControllerBase
{
    private readonly IExportationService _exportationService;
    private readonly IBlogGenerator _blogGenerator;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        IExportationService exportationService,
        IBlogGenerator blogGenerator,
        ILogger<ExportController> logger
    )
    {
        _exportationService = exportationService;
        _blogGenerator = blogGenerator;
        _logger = logger;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        await _exportationService.ExportAsync(cancellationToken);

        _logger.LogInformation("Exported successfully");
        return NoContent();
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview(CancellationToken cancellationToken)
    {
        var preview = await _blogGenerator.GeneratePreviewAsync(cancellationToken);

        return Content(preview, "text/html");
    }
}