using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.OrderConstants;

namespace LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly
{
    public class OrderInformationDto
    {
        [Required]
        [MinLength(OrderCountryMinLength)]
        [MaxLength(OrderCountryMaxLength)]
        public string Country { get; set; } = null!;

        [Required]
        [MinLength(OrderCityMinLength)]
        [MaxLength(OrderCityMaxLength)]
        public string City { get; set; } = null!;

        [Required]
        [MinLength(OrderStreetMinLength)]
        [MaxLength(OrderStreetMaxLength)]
        public string Street { get; set; } = null!;

        [Required]
        [MinLength(OrderPostalCodeMinLength)]
        [MaxLength(OrderPostalCodeMaxLength)]
        public string PostalCode { get; set; } = null!;

        [MaxLength(OrderAdditionalInformationMaxLength)]
        public string? AdditionalInformation { get; set; }
    }
}
