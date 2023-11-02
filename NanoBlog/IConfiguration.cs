namespace NanoBlog;

public interface IConfiguration
{
    /* for simplicity reasons, those values are not configurable itself */
    public const string INDEX_FILE_NAME = "index.html";
    public const string ASSETS_DIRECTORY_NAME = "assets";
    public const string ARCHIVE_DIRECTORY_NAME = "archive";
    public const string ARCHIVE_INDEX_FORMAT = "D";

    public const string STAGE_STRUCTURE_FILE_NAME_HEADER = "header.txt";
    public const string STAGE_STRUCTURE_FILE_NAME_INTRO = "intro.txt";
    public const string STAGE_STRUCTURE_FILE_NAME_LEGAL = "legal.txt";
    public const string STAGE_STRUCTURE_FILE_NAME_FOOTER = "footer.txt";

    protected const string STAGE_ASSETS_DIRECTORY_NAME = "BlogFiles/Assets";
    protected const string STAGE_POSTS_DIRECTORY_NAME = "BlogFiles/Posts";
    protected const string STAGE_STRUCTURE_DIRECTORY_NAME = "BlogFiles/Structure";
    protected const string EXPORT_DIRECTORY_NAME = "Export";

    protected const string DEFAULT_LANGUAGE = "en";
    protected const string DEFAULT_BLOG_ROOT_SERVER_DIRECTORY = "/";
    protected const bool DEFAULT_USE_PAGINATION = false;
    protected const int DEFAULT_PAGE_SIZE = 0;

    public bool UsePagination { get; }
    public int PageSize { get; }
    public string Language { get; }
    public string BlogRootServerDirectory { get; }
    public ICollection<string> ExportKeepFileNames { get; }
}