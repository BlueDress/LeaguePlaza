using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly
{
    public class OrderInformationDto
    {
        [Required]
        [MaxLength(200)]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = null!;

        [MaxLength(200)]
        public string? AdditionalInformation { get; set; }
    }
}
