namespace NanoBlog.Services.FileStorages;

public interface IFileStorage
{
    IEnumerable<FileInfo> GetFileInfos();
    bool FileExists(string fileName);

    FileStream OpenReadStream(string fileName);
    FileStream? TryOpenReadStream(string fileName);
    FileStream? TryOpenWriteStream(string fileName);

    Task<string> LoadContentAsStringAsync(FileStream fileStream, CancellationToken cancellationToken);
    Task<byte[]> LoadContentAsBytesAsync(FileStream fileStream, CancellationToken cancellationToken);

    void Delete(string fileName);
    void Truncate();
}