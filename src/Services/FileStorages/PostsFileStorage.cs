namespace NanoBlog.Services.FileStorages;

public class PostsFileStorage : AbstractFileStorage, IPostsFileStorage
{
    protected override string BaseFolder => "BlogFiles/Posts";

    public async Task<string> CreatePostFileAsync(Stream content, CancellationToken cancellationToken)
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.txt";
        var targetFilePath = Path.Combine(BaseFolder, fileName);

        await using var fileHandle = File.Open(targetFilePath, FileMode.CreateNew);
        await content.CopyToAsync(fileHandle, cancellationToken);

        return fileName;
    }

    public void Delete(string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }
        
        File.Delete(filePath);
    }
}