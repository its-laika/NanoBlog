using System.Text;
using Microsoft.AspNetCore.Mvc;
using NanoBlog.Services;
using NanoBlog.Services.FileStorages;

namespace NanoBlog.Controllers;

[ApiController]
public class ExportController : ControllerBase
{
    private readonly IExportFileStorage _fileStorage;
    private readonly IBlogGenerator _blogGenerator;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        IExportFileStorage fileStorage,
        IBlogGenerator blogGenerator,
        ILogger<ExportController> logger
    )
    {
        _fileStorage = fileStorage;
        _blogGenerator = blogGenerator;
        _logger = logger;
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export(CancellationToken cancellationToken)
    {
        var generatedContent = await _blogGenerator.GenerateContentAsync(cancellationToken);
        var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(generatedContent));
        await _fileStorage.WriteContentAsync(IExportFileStorage.ExportFileName, contentStream, cancellationToken);

        _logger.LogInformation("Exported successfully");
        
        return Ok();
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview(CancellationToken cancellationToken)
    {
        var generatedContent = await _blogGenerator.GenerateContentAsync(cancellationToken);
        
        return Ok(generatedContent);
    }
}