using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Order.Models.Dtos.Create
{
    public class CreateCartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
