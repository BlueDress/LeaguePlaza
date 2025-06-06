using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateImageFileSignatureAttribute : ValidationAttribute
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
                    return new ValidationResult($"File type is not allowed.");
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
