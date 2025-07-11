using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Admin.Models.ViewModels
{
    public class ProductAdminViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; } = new HashSet<ProductDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
