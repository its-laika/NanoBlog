namespace NanoBlog.Services.MimeTypes;

public class MimeTypeProvider : IMimeTypeProvider
{
    private readonly IReadOnlyList<(byte[] Sequence, MimeType MimeType, string Extension)> _magicByteMapping =
        new List<(byte[], MimeType, string)>
        {
            (new byte[] { 0xFF, 0xD8, 0xFF }, MimeType.Jpeg, "jpeg"),
            (new byte[] { 0xFF, 0xD8, 0xFF }, MimeType.Jpeg, "jpg"),
            (new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, MimeType.Png, "png"),
            ("GIF87a"u8.ToArray(), MimeType.Gif, "gif"),
            ("GIF89a"u8.ToArray(), MimeType.Gif, "gif"),
            ("<?xml"u8.ToArray(), MimeType.Svg, "svg"),
        };

    public async Task<MimeType?> ProvideMimeTypeAsync(
        IFormFile formFile,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        var detectedMimeType = await ProvideMimeTypeAsync(formFile.FileName, content, cancellationToken);
        if (formFile.ContentType is not { Length: > 0 } fileMimeType)
        {
            return detectedMimeType;
        }

        return detectedMimeType?.AsString().Equals(fileMimeType, StringComparison.InvariantCultureIgnoreCase) == true
            ? detectedMimeType
            : null;
    }

    public async Task<MimeType?> ProvideMimeTypeAsync(
        string fileName,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        var mimeTypesByFileExtension = DetermineMimeType(fileName);
        var mimeTypesByContent = await DetermineMimeTypesAsync(content, cancellationToken);

        if (mimeTypesByFileExtension.Intersect(mimeTypesByContent).ToList() is { Count: 1 } distinctMimeTypes)
        {
            return distinctMimeTypes.First();
        }

        return null;
    }

    private IEnumerable<MimeType> DetermineMimeType(string fileName)
    {
        var fileExtension = fileName.Split('.').Last();

        return _magicByteMapping
            .Where(m => m.Extension.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
            .Select(m => m.MimeType)
            .Distinct()
            .ToList();
    }

    private async Task<IReadOnlyList<MimeType>> DetermineMimeTypesAsync(
        Stream fileStream,
        CancellationToken cancellationToken
    )
    {
        var bufferSize = _magicByteMapping.Max(m => m.Sequence.Length);
        var buffer = new byte[bufferSize];

        var copiedBytes = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        fileStream.Position -= copiedBytes;

        return _magicByteMapping
            .Where(mb => mb.Sequence.Length <= copiedBytes)
            .OrderBy(mb => mb.Sequence.Length)
            .Where(m => buffer.Take(m.Sequence.Length).SequenceEqual(m.Sequence))
            .Select(m => m.MimeType)
            .Distinct()
            .ToList();
    }
}