namespace NanoBlog.Containers;

internal class StageDirectoryContainer : IStageDirectoryContainer
{
    public DirectoryInfo AssetsDirectory { get; }
    public DirectoryInfo ExportDirectory { get; }
    public DirectoryInfo PostsDirectory { get; }
    public DirectoryInfo StructureDirectory { get; }

    public StageDirectoryContainer()
    {
        AssetsDirectory = EnsureExisting(Configuration.GetStageAssetsDirectoryInfo());
        ExportDirectory = EnsureExisting(Configuration.GetExportDirectoryInfo());
        PostsDirectory = EnsureExisting(Configuration.GetStagePostsDirectoryInfo());
        StructureDirectory = EnsureExisting(Configuration.GetStageStructureDirectoryInfo());
    }

    private static DirectoryInfo EnsureExisting(DirectoryInfo directoryInfo)
    {
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        return directoryInfo.EnsureSecureMode();
    }
}