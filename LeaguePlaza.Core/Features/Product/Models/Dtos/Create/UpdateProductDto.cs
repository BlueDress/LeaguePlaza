using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Product.Models.Dtos.Create
{
    public class UpdateProductDto : CreateProductDto
    {
        [Required]
        public int Id { get; set; }
    }
}
