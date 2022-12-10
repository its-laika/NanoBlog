namespace NanoBlog.Services.FileStorages;

public class StructureFileStorage : AbstractFileStorage, IStructureFileStorage
{
    protected override string BaseFolder => "BlogFiles/Structure";
}