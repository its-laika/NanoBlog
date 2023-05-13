namespace NanoBlog;

public interface IConfiguration
{
    public bool UsePagination { get; }
    public int PageSize { get; }
    public string Language { get; }
}