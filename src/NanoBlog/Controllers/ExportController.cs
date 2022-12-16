using System.Text;
using Microsoft.AspNetCore.Mvc;
using NanoBlog.Services;
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
        var generatedContent = await _blogGenerator.GenerateContentAsync(cancellationToken);

        var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(generatedContent));
        await _exportFileStorage.WriteContentAsync(contentStream, cancellationToken);
        _logger.LogInformation("Exported successfully");

        await _assetsFileStorage.TransferAsync(cancellationToken);
        _logger.LogInformation("Transferred assets successfully");

        return Ok();
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview(CancellationToken cancellationToken)
    {
        var generatedContent = await _blogGenerator.GenerateContentAsync(cancellationToken);
        return Ok(generatedContent);
    }
}