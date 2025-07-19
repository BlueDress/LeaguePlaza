using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Order.Models.Dtos.Create
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Status { get; set; } = null!;
    }
}
