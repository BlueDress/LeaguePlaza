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

        public async Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync(int pageNumber = OrderConstants.PageOne)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new OrderHistoryViewModel();
            }

            IEnumerable<OrderEntity> orders = await _repository.FindSpecificCountOrderedReadOnlyAsync<OrderEntity, DateTime?>(pageNumber, OrderConstants.CountForOrderHistoryPagination, true, o => o.DateCompleted, o => o.UserId == currentUser.Id);
            int totalResults = await _repository.GetCountAsync<OrderEntity>(o => o.UserId == currentUser.Id);

            return new OrderHistoryViewModel()
            {
                Orders = orders.Select(o => new OrderDto()
                {
                    Id = o.Id,
                    DateCreated = o.DateCreated.ToString("dd.MM.yyyy"),
                    DateCompleted = o.DateCompleted.HasValue ? o.DateCompleted.Value.ToString("dd.MM.yyyy") : string.Empty,
                    Status = o.Status.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(totalResults / 10d),
                },
            };
        }

        public async Task<CartViewModel> CreateViewCartViewModelAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new CartViewModel();
            }

            CartEntity? userCart = await _repository.FindOneReadOnlyAsync<CartEntity>(c => c.UserID == currentUser.Id, query => query.Include(c => c.CartItems).ThenInclude(ci => ci.Product));

            if (userCart == null)
            {
                await _repository.AddAsync(new CartEntity() { UserID = currentUser.Id });
                await _repository.SaveChangesAsync();
                return new CartViewModel();
            }

            return new CartViewModel()
            {
                CartId = userCart.Id,
                TotalPrice = userCart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                CartItems = userCart.CartItems.Select(ci => new CartItemDto()
                {
                    Id = ci.Id,
                    Quantity = ci.Quantity,
                    ProductName = ci.Product.Name,
                    ProductPrice = ci.Product.Price,
                    TotalPrice = ci.Quantity * ci.Product.Price,
                }),
            };
        }

        public async Task<int> GetCartItemsCountAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return 0;
            }
            CartEntity? userCart = await _repository.FindOneReadOnlyAsync<CartEntity>(c => c.UserID == currentUser.Id);

            if (userCart == null)
            {
                await _repository.AddAsync(new CartEntity() { UserID = currentUser.Id });
                await _repository.SaveChangesAsync();
                return 0;
            }

            return await _repository.GetCountAsync<CartItemEntity>(ci => ci.CartId == userCart.Id);
        }

        public async Task<OrderViewModel> CreateOrderViewModelAsync(int orderId)
        {
            var order = await _repository.FindOneReadOnlyAsync<OrderEntity>(o => o.Id == orderId, query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product));

            if (order == null)
            {
                return new OrderViewModel();
            }

            return new OrderViewModel()
            {
                Order = new OrderDto()
                {
                    Id = order.Id,
                    DateCreated = order.DateCreated.ToString("dd.MM.yyyy"),
                    DateCompleted = order.DateCompleted.HasValue ? order.DateCompleted.Value.ToString("dd.MM.yyyy") : null,
                    Status = order.Status.ToString(),
                },
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto()
                {
                    ProductName = oi.Product.Name,
                    ProductImageUrl = oi.Product.ImageUrl,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                }),
            };
        }
    }
}
