namespace NanoBlog.Services.FileStorages.Assets;

public interface IAssetsFileStorage : IFileStorage
{
    FileStream CreateNewFileWriteStreamByMimeType(MimeType mimeType);
}