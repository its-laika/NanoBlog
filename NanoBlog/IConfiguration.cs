namespace NanoBlog;

public interface IConfiguration
{
    public DirectoryInfo GetAssetsDirectoryInfo();
    public DirectoryInfo GetPostsDirectoryInfo();

    public DirectoryInfo GetExportDirectoryInfo();
    public DirectoryInfo GetExportAssetsDirectoryInfo();
    public DirectoryInfo GetExportArchiveDirectoryInfo();

    public string PagePlaceholderPosts { get; }
    public string PagePlaceholderNavigation { get; }
    public string PostPlaceholderContent { get; }
    public string PostPlaceholderName { get; }
    public string PostPlaceholderDate { get; }
    public string PostDateFormat { get; }

    public string PageTemplate { get; }
    public string PostTemplate { get; }
    public int? PageSize { get; }

    public ICollection<string> KeepExportFiles { get; }
}