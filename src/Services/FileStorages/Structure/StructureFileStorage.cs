namespace NanoBlog.Services.FileStorages.Structure;

public class StructureFileStorage : AbstractFileStorage, IStructureFileStorage
{
    protected override string BaseFolder { get; }

    public StructureFileStorage(ILogger<AbstractFileStorage> logger): base(logger)
    {
        BaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "BlogFiles/Structure");
    }
}