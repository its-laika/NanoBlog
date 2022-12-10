using System.ComponentModel.DataAnnotations;

namespace NanoBlog.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class ValidFileName : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string fileName)
        {
            return false;
        }

        return !fileName.Contains('/') && !fileName.Contains("..");
    }
}