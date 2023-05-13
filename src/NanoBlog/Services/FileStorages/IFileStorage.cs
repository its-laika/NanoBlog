namespace NanoBlog.Services.FileStorages;

public interface IFileStorage
{
    IEnumerable<string> GetFileNames();
    bool FileExists(string fileName);

    FileStream OpenReadStream(string fileName);
    FileStream? TryOpenReadStream(string fileName);
    FileStream? TryOpenWriteStream(string fileName);

    Task<string> LoadContentAsync(FileStream fileStream, CancellationToken cancellationToken);

    void Delete(string fileName);

    FileStream CreateWriteStream();
}