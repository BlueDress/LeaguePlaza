using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxFileSizeAttribute(int maxFileSize) : ValidationAttribute
    {
        private readonly int _maxFileSize = maxFileSize;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {_maxFileSize} bytes.";
        }
    }
}
