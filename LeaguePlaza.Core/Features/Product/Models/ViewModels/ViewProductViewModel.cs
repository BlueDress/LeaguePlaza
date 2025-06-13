using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Product.Models.ViewModels
{
    public class ViewProductViewModel
    {
        public ProductDto Product { get; set; } = new ProductDto();

        public IEnumerable<ProductDto> RecommendedProducts { get; set; } = new List<ProductDto>();
    }
}
