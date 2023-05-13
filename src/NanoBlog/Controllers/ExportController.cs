using Microsoft.AspNetCore.Mvc;
using NanoBlog.Services.Generation;
using NanoBlog.Services.FileStorages.Assets;
using NanoBlog.Services.FileStorages.Export;

namespace NanoBlog.Controllers;

[ApiController]
public class ExportController : ControllerBase
{
    private readonly IExportFileStorage _exportFileStorage;
    private readonly IAssetsFileStorage _assetsFileStorage;
    private readonly IBlogGenerator _blogGenerator;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        IExportFileStorage exportFileStorage,
        IAssetsFileStorage assetsFileStorage,
        IBlogGenerator blogGenerator,
        ILogger<ExportController> logger
    )
    {
        _exportFileStorage = exportFileStorage;
        _assetsFileStorage = assetsFileStorage;
        _blogGenerator = blogGenerator;
        _logger = logger;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var pageMapping = await _blogGenerator.GeneratePageMappingAsync(cancellationToken);

        await _assetsFileStorage.SynchronizeFilesAsync(cancellationToken);
        await _exportFileStorage.WriteContentsAsync(pageMapping, cancellationToken);

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