namespace NanoBlog.Containers;

public interface IStageDirectoryContainer
{
    public DirectoryInfo AssetsDirectory { get; }
    public DirectoryInfo ExportDirectory { get; }
    public DirectoryInfo PostsDirectory { get; }
    public DirectoryInfo StructureDirectory { get; }
}