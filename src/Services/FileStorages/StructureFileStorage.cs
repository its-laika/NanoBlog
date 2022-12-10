namespace NanoBlog.Services.FileStorages;

public class StructureFileStorage : AbstractFileStorage, IStructureFileStorage
{
    protected override string BaseFolder { get; }

    public StructureFileStorage()
    {
        BaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "BlogFiles/Structure");
    }
}