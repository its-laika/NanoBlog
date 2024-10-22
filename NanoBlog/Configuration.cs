// ReSharper disable MemberCanBePrivate.Global

namespace NanoBlog;

public class Configuration : IConfiguration
{
    public string PagePlaceholderPosts { get; init; } = string.Empty;
    public string PagePlaceholderNavigation { get; init; } = string.Empty;
    public string PostPlaceholderContent { get; init; } = string.Empty;
    public string PostPlaceholderName { get; init; } = string.Empty;

    public bool UsePagination { get; init; } = false;
    public string PageTemplate { get; init; } = string.Empty;
    public string PostTemplate { get; init; } = string.Empty;
    public int? PageSize { get; init; } = null;

    public string PostDirectory { get; init; } = string.Empty;
    public string AssetDirectory { get; init; } = string.Empty;

    public string ExportDirectory { get; init; } = string.Empty;

    public ICollection<string> KeepExportFiles { get; init; } = [];

    public DirectoryInfo GetAssetsDirectoryInfo()
    {
        EnsureNotEmpty(AssetDirectory);
        return EnsureExisting(Path.Combine(Directory.GetCurrentDirectory(), AssetDirectory));
    }

    public DirectoryInfo GetPostsDirectoryInfo()
    {
        EnsureNotEmpty(PostDirectory);
        return EnsureExisting(Path.Combine(Directory.GetCurrentDirectory(), PostDirectory));
    }

    public DirectoryInfo GetExportDirectoryInfo()
    {
        EnsureNotEmpty(ExportDirectory);
        return EnsureExisting(Path.Combine(Directory.GetCurrentDirectory(), ExportDirectory));
    }

    public DirectoryInfo GetExportAssetsDirectoryInfo()
    {
        EnsureNotEmpty(ExportDirectory);
        return EnsureExisting(Path.Combine(Directory.GetCurrentDirectory(), ExportDirectory, "assets"));
    }

    public DirectoryInfo GetExportArchiveDirectoryInfo()
    {
        EnsureNotEmpty(ExportDirectory);
        return EnsureExisting(Path.Combine(Directory.GetCurrentDirectory(), ExportDirectory, "archive"));
    }

    private static DirectoryInfo EnsureExisting(string path)
    {
        var directoryInfo = new DirectoryInfo(path);

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        return directoryInfo.EnsureSecureMode();
    }

    private static void EnsureNotEmpty(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        throw new Exception("Path not configured");
    }
}