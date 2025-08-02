using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.ErrorConstants;

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
                    var logger = validationContext.GetService<ILogger<MaxFileSizeAttribute>>();

                    logger?.LogError(FailedAt, nameof(IsValid));
                    logger?.LogError(MaxFileSizeErrorMessage, file.Length);

                    return new ValidationResult(GenericErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
