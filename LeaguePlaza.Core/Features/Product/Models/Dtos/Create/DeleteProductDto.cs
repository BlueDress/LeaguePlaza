using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Product.Models.Dtos.Create
{
    public class DeleteProductDto
    {
        [Required]
        public int Id { get; set; }
    }
}
