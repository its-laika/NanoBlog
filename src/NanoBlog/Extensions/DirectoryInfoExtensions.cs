namespace NanoBlog.Extensions;

public static class DirectoryInfoExtensions
{
    private const UnixFileMode _DIRECTORY_MODE =
        UnixFileMode.UserRead
        | UnixFileMode.UserWrite
        | UnixFileMode.UserExecute
        | UnixFileMode.GroupRead
        | UnixFileMode.GroupExecute
        | UnixFileMode.OtherRead
        | UnixFileMode.OtherExecute;

    public static FileInfo? TryFindFileInfo(this DirectoryInfo directoryInfo, string fileName)
    {
        directoryInfo.Refresh();

        return directoryInfo
           .EnumerateFiles()
           .Where(f => f.Exists) /* this is possibly dumb */
           .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));
    }

    public static FileInfo FindFileInfo(this DirectoryInfo directoryInfo, string fileName)
    {
        return directoryInfo.TryFindFileInfo(fileName)
            ?? throw new FileNotFoundException($"Could not find file {fileName}");
    }

    public static bool HasFile(this DirectoryInfo directoryInfo, string fileName)
    {
        return directoryInfo.TryFindFileInfo(fileName) is not null;
    }

    public static FileStream CreateFile(this DirectoryInfo directoryInfo, string fileName)
    {
        var fileNameValidator = new ValidFileName.All();
        if (!fileNameValidator.IsValid(fileName))
        {
            throw new ArgumentException($"File name {fileName} is not valid");
        }

        directoryInfo.Refresh();
        var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, fileName));

        if (!fileInfo.FullName.StartsWith(directoryInfo.FullName, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ArgumentOutOfRangeException($"File {fileName} is not in parent directory");
        }

        if (fileInfo.Exists)
        {
            throw new ArgumentException($"File {fileInfo.FullName} already exists");
        }

        var fileStream = fileInfo.Create();
        fileInfo.EnsureFileMode();

        return fileStream;
    }

    public static void Clear(this DirectoryInfo directoryInfo)
    {
        directoryInfo.Refresh();

        foreach (var fileInfo in directoryInfo.EnumerateFiles())
        {
            fileInfo.Delete();
        }

        foreach (var subDirectoryInfo in directoryInfo.EnumerateDirectories())
        {
            Directory.Delete(subDirectoryInfo.FullName, true);
        }
    }

    public static void DeleteFile(this DirectoryInfo directoryInfo, string fileName)
    {
        directoryInfo.FindFileInfo(fileName).Delete();
    }

    public static DirectoryInfo EnsureSecureMode(this DirectoryInfo directoryInfo)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            directoryInfo.UnixFileMode = _DIRECTORY_MODE;
        }

        return directoryInfo;
    }
}