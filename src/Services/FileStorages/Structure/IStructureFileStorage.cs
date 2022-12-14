namespace NanoBlog.Services.FileStorages.Structure;

public interface IStructureFileStorage : IFileStorage
{
    public const string FileNameHtmlHeader = "html-header.txt";
    public const string FileNameHeader = "header.txt";
    public const string FileNameFooter = "footer.txt";
}