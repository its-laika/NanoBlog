using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NanoBlog.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class ValidFileName : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string fileName
               && Regex.IsMatch(fileName, "^[A-Za-z0-9\\-]+\\.txt$");
    }
}