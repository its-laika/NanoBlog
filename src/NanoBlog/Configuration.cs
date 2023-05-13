// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace NanoBlog;

public class Configuration : IConfiguration
{
    public bool UsePagination { get; init; } = false;
    public int PageSize { get; init; } = 0;
    public string Language { get; init; } = "en";
}