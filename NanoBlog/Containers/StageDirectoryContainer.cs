namespace NanoBlog.Containers;

internal class StageDirectoryContainer : IStageDirectoryContainer
{
    public DirectoryInfo AssetsDirectory { get; }
        = EnsureExisting(Configuration.GetStageAssetsDirectoryInfo());

    public DirectoryInfo ExportDirectory { get; }
        = EnsureExisting(Configuration.GetExportDirectoryInfo());

    public DirectoryInfo PostsDirectory { get; }
        = EnsureExisting(Configuration.GetStagePostsDirectoryInfo());

    public DirectoryInfo StructureDirectory { get; }
        = EnsureExisting(Configuration.GetStageStructureDirectoryInfo());

    private static DirectoryInfo EnsureExisting(DirectoryInfo directoryInfo)
    {
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        return directoryInfo.EnsureSecureMode();
    }
}