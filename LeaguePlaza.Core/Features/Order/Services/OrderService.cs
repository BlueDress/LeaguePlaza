using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaguePlaza.Core.Features.Order.Services
{
    public class OrderService(IRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) : IOrderService
    {
        private readonly IRepository _repository = repository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new OrderHistoryViewModel();
            }

            IEnumerable<OrderEntity> orders = await _repository.FindSpecificCountOrderedReadOnlyAsync<OrderEntity, DateTime>(OrderConstants.PageOne, OrderConstants.CountForOrderHistoryPagination, true, o => o.DateCreated, o => o.UserId == currentUser.Id, query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product));
            int totalResults = await _repository.GetCountAsync<OrderEntity>(o => o.UserId == currentUser.Id);

            return new OrderHistoryViewModel()
            {
                Orders = orders.Select(o => new OrderDto()
                {
                    Id = o.Id,
                    DateCreated = o.DateCreated.ToString("dd-MM-yyyy"),
                    DateCompleted = o.DateCompleted.HasValue ? o.DateCompleted.Value.ToString("dd-MM-yyyy") : string.Empty,
                    Status = o.Status.ToString(),
                    TotalPrice = o.OrderItems.Sum(oi => oi.Price),
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto()
                    {
                        ProductName = oi.Product.Name,
                        ProductImageUrl = oi.Product.ImageUrl,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                    }),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = QuestConstants.PageOne,
                    TotalPages = (int)Math.Ceiling(totalResults / 10d),
                },
            };
        }
    }
}
