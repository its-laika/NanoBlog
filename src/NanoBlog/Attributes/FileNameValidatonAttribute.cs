using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NanoBlog.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public partial class ValidFileName : ValidationAttribute
{
    public class Text : ValidFileName
    {
        public Text() : base(TextRegex())
        {
        }
    }

    public class Asset : ValidFileName
    {
        public Asset() : base(AssetRegex())
        {
        }
    }

    [GeneratedRegex("^[A-Za-z0-9\\-]+\\.(png|jpg|jpeg|gif)$")]
    private static partial Regex AssetRegex();

    [GeneratedRegex("^[A-Za-z0-9\\-]+\\.txt$")]
    private static partial Regex TextRegex();

    private const int _MAX_FILE_LENGTH = 100;
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

        if (fileName.Length > _MAX_FILE_LENGTH)
        {
            return new ValidationResult($"File name exceeded max length ({_MAX_FILE_LENGTH}).");
        }

        return _validFileNameRegex.IsMatch(fileName)
            ? ValidationResult.Success
            : new ValidationResult($"File name '{fileName}' is not valid (should match {_validFileNameRegex})");
    }
}