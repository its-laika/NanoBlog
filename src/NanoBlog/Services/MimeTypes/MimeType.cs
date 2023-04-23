namespace NanoBlog.Services.MimeTypes;

public enum MimeType
{
    Jpeg,
    Png,
    Gif,
    Svg
}

public static class MimeTypeExtensions
{
    public static string AsString(this MimeType mimeType)
    {
        return mimeType switch
        {
            MimeType.Gif => "image/gif",
            MimeType.Png => "image/png",
            MimeType.Jpeg => "image/jpeg",
            MimeType.Svg => "image/svg+xml",
            _ => throw new ArgumentOutOfRangeException(nameof(mimeType), mimeType, null)
        };
    }

    public static string GetExtension(this MimeType mimeType)
    {
        return mimeType switch
        {
            MimeType.Gif => "gif",
            MimeType.Png => "png",
            MimeType.Jpeg => "jpeg", /* Should be valid for all kinds of JPEG */
            MimeType.Svg => "svg",
            _ => throw new ArgumentOutOfRangeException(nameof(mimeType), mimeType, null)
        };
    }
}