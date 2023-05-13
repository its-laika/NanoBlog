namespace NanoBlog.Services.FileStorages.Structure;

public interface IStructureFileStorage : IFileStorage
{
    public const string FILE_NAME_HEADER = "header.txt";
    public const string FILE_NAME_INTRO = "intro.txt";
    public const string FILE_NAME_LEGAL = "legal.txt";
    public const string FILE_NAME_FOOTER = "footer.txt";
}