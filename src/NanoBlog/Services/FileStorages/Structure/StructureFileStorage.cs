namespace NanoBlog.Services.FileStorages.Structure;

public class StructureFileStorage : AbstractFileStorage, IStructureFileStorage
{
    public StructureFileStorage(
        IFileSystemSecurityService fileSystemSecurityService
    ) : base(
        fileSystemSecurityService,
        Configuration.GetStageStructureDirectoryInfo()
    )
    {
    }
}