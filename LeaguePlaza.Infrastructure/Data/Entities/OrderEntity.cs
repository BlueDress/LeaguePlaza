using LeaguePlaza.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static LeaguePlaza.Common.Constants.OrderConstants;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        [MaxLength(OrderCountryMaxLength)]
        public string Country { get; set; } = null!;

        [Required]
        [MaxLength(OrderCityMaxLength)]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(OrderStreetMaxLength)]
        public string Street { get; set; } = null!;

        [Required]
        [MaxLength(OrderPostalCodeMaxLength)]
        public string PostalCode { get; set; } = null!;

        [MaxLength(OrderAdditionalInformationMaxLength)]
        public string? AdditionalInformation { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<OrderItemEntity> OrderItems { get; set; } = new HashSet<OrderItemEntity>();
    }
}
