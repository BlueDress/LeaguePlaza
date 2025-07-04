using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.ErrorConstants;

namespace LeaguePlaza.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateImageFileSignatureAttribute() : ValidationAttribute
    {
        private static readonly List<byte[]> ImageFileSignatures = new()
        {
            { new byte[] { 0xFF, 0xD8, 0xFF } }, // jpg, jpeg
            { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } }, // png
            { new byte[] { 0x47, 0x49, 0x46, 0x38 } }, // gif
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (!IsFileValid(file))
                {
                    var logger = validationContext.GetService<ILogger<ValidateImageFileSignatureAttribute>>();

                    logger?.LogError(FailedAt, nameof(IsValid));
                    logger?.LogError(ValidateImageFileSignatureErrorMessage);

                    return new ValidationResult(GenericErrorMessage);
                }
            }

            return ValidationResult.Success;
        }

        public static bool IsFileValid(IFormFile file)
        {
            using var reader = new BinaryReader(file.OpenReadStream());
            var headerBytes = reader.ReadBytes(ImageFileSignatures.Max(s => s.Length));
            return ImageFileSignatures.Any(s => headerBytes.Take(s.Length).SequenceEqual(s));
        }
    }
}
