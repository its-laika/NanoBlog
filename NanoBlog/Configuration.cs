// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace NanoBlog;

public class Configuration : IConfiguration
{
    public bool UsePagination { get; init; } = IConfiguration.DEFAULT_USE_PAGINATION;
    public int PageSize { get; init; } = IConfiguration.DEFAULT_PAGE_SIZE;
    public string Language { get; init; } = IConfiguration.DEFAULT_LANGUAGE;
    public string BlogRootServerDirectory { get; init; } = IConfiguration.DEFAULT_BLOG_ROOT_SERVER_DIRECTORY;

    public static DirectoryInfo GetStageAssetsDirectoryInfo()
    {
        return new DirectoryInfo(
            Path.Combine(Directory.GetCurrentDirectory(), IConfiguration.STAGE_ASSETS_DIRECTORY_NAME)
        );
    }

    public static DirectoryInfo GetStagePostsDirectoryInfo()
    {
        return new DirectoryInfo(
            Path.Combine(Directory.GetCurrentDirectory(), IConfiguration.STAGE_POSTS_DIRECTORY_NAME)
        );
    }

    public static DirectoryInfo GetStageStructureDirectoryInfo()
    {
        return new DirectoryInfo(
            Path.Combine(Directory.GetCurrentDirectory(), IConfiguration.STAGE_STRUCTURE_DIRECTORY_NAME)
        );
    }

    public static DirectoryInfo GetExportDirectoryInfo()
    {
        return new DirectoryInfo(
            Path.Combine(Directory.GetCurrentDirectory(), IConfiguration.EXPORT_DIRECTORY_NAME)
        );
    }
}