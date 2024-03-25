namespace NanoBlog.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex(@"\s{1,}")]
    private static partial Regex NormalizeWhitespaceRegex();

    public static string Truncate(this string subject, int length)
    {
        return subject.Length <= length 
            ? subject 
            : subject[..length];
    }

    public static string NormalizeWhitespaces(this string subject)
    {
        return NormalizeWhitespaceRegex()
           .Replace(subject, " ");
    }
}