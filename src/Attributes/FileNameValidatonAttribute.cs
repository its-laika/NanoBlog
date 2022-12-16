using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NanoBlog.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class ValidFileName : ValidationAttribute
{
    private const int _MAX_FILE_LENGTH = 100;
    private readonly Regex _validationRegex = new("^[A-Za-z0-9\\-]+\\.txt$");

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string fileName)
        {
            return new ValidationResult("Given file name is not of type string");
        }

        if (fileName.Length > _MAX_FILE_LENGTH)
        {
            return new ValidationResult($"File name exceeded max length ({_MAX_FILE_LENGTH}).");
        }

        return !_validationRegex.IsMatch(fileName)
            ? new ValidationResult($"Given file name does not match structure {_validationRegex}")
            : ValidationResult.Success;
    }
}