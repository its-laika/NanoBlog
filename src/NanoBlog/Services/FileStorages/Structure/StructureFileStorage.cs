using NanoBlog.Services.FileSystemSecurity;

namespace NanoBlog.Services.FileStorages.Structure;

public class StructureFileStorage : AbstractFileStorage, IStructureFileStorage
{
    public StructureFileStorage(IFileSystemSecurityService fileSystemSecurityService) : base(
        fileSystemSecurityService,
        new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "BlogFiles/Structure"))
    )
    {
    }
}