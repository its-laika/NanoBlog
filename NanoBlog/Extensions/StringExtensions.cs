namespace NanoBlog.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string subject, int length)
    {
        return subject.Length <= length 
            ? subject 
            : subject[..length];
    }
}