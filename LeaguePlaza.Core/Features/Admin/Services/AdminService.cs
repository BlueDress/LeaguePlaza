using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

using static LeaguePlaza.Common.Constants.MountConstants;
using static LeaguePlaza.Common.Constants.ProductConstants;
using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Admin.Services
{
    public class AdminService(IRepository repository) : IAdminService
    {
        private readonly IRepository _repository = repository;

        public async Task<MountAdminViewModel> CreateMountAdminViewModelAsync(int pageNumber = PageOne)
        {
            IEnumerable<MountEntity> mounts = await _repository.FindSpecificCountOrderedReadOnlyAsync<MountEntity, int>(pageNumber, AdminCountForPagination, false, m => m.Id, m => true);
            int totalResults = await _repository.GetCountAsync<MountEntity>(m => true);

            return new MountAdminViewModel()
            {
                Mounts = mounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? NoMountDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalResults / AdminCountForPagination),
                },
            };
        }

        public async Task<ProductAdminViewModel> CreateProductAdminViewModelAsync(int pageNumber = 1)
        {
            IEnumerable<ProductEntity> products = await _repository.FindSpecificCountOrderedReadOnlyAsync<ProductEntity, int>(pageNumber, AdminCountForPagination, false, p => p.Id, p => true);
            int totalResults = await _repository.GetCountAsync<ProductEntity>(p => true);

            return new ProductAdminViewModel()
            {
                Products = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? NoProductDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalResults / AdminCountForPagination),
                },
            };
        }

        public async Task<OrderAdminViewModel> CreateOrderAdminViewModelAsync(int pageNumber = 1)
        {
            IEnumerable<OrderEntity> orders = await _repository.FindSpecificCountOrderedReadOnlyAsync<OrderEntity, int>(pageNumber, 2, false, o => o.Id, o => true);
            int totalResults = await _repository.GetCountAsync<OrderEntity>(o => true);

            return new OrderAdminViewModel()
            {
                Orders = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    DateCreated = o.DateCreated.ToString("dd.MM.yyyy"),
                    DateCompleted = o.DateCompleted.HasValue ? o.DateCompleted.Value.ToString("dd.MM.yyyy") : string.Empty,
                    Status = o.Status.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalResults / 2),
                },
            };
        }
    }
}
