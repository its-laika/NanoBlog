namespace NanoBlog.Extensions;

public static class FileStreamExtensions
{
    public static async Task<string> LoadAsStringAsync(this FileStream fileStream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(fileStream);

        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public static async Task<byte[]> LoadAsBytesAsync(this FileStream fileStream, CancellationToken cancellationToken)
    {
        var buffer = new byte[fileStream.Length];

        _ = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);

        return [.. buffer];
    }
}