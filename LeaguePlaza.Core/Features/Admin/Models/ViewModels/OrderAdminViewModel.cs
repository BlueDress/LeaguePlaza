using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;

namespace LeaguePlaza.Core.Features.Admin.Models.ViewModels
{
    public class OrderAdminViewModel
    {
        public IEnumerable<OrderDto> Orders { get; set; } = new HashSet<OrderDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
