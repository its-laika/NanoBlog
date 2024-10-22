namespace NanoBlog.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public partial class ValidFileName : ValidationAttribute
{
    public class Text() : ValidFileName(TextRegex());

    public class Asset() : ValidFileName(AssetRegex());

    public class All() : ValidFileName(AllRegex());

    [GeneratedRegex(@"^[A-Za-z0-9\-]+\.(png|jpg|jpeg|gif|svg)$")]
    private static partial Regex AssetRegex();

    [GeneratedRegex(@"^[A-Za-z0-9\-]+\.txt$")]
    private static partial Regex TextRegex();

    [GeneratedRegex(@"^[A-Za-z0-9\-]+\.(png|jpg|jpeg|gif|svg|txt|html)")]
    private static partial Regex AllRegex();

    private const int MaxFileLength = 100;
    private readonly Regex _validFileNameRegex;

    private ValidFileName(Regex validFileNameRegex)
    {
        _validFileNameRegex = validFileNameRegex;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string fileName)
        {
            return new ValidationResult($"Attributed value is not of type {typeof(string)}");
        }

        if (fileName.Length > MaxFileLength)
        {
            return new ValidationResult($"File name exceeded max length ({MaxFileLength}).");
        }

        return _validFileNameRegex.IsMatch(fileName)
            ? ValidationResult.Success
            : new ValidationResult($"File name '{fileName}' is not valid (should match {_validFileNameRegex})");
    }
}