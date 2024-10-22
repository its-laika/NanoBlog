namespace NanoBlog.Controllers;

[ApiController]
public class ExportController(IExportService exportService) : ControllerBase
{
    [HttpPost("export")]
    public async Task<IActionResult> ExportAsync(CancellationToken cancellationToken)
    {
        await exportService.ExportAsync(cancellationToken);

        return NoContent();
    }
}