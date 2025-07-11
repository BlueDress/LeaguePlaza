﻿using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static LeaguePlaza.Common.Constants.OrderConstants;
using static LeaguePlaza.Common.Constants.PaginationConstants;
using static LeaguePlaza.Common.Constants.ErrorConstants;

namespace LeaguePlaza.Core.Features.Order.Services
{
    public class OrderService(IRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) : IOrderService
    {
        private readonly IRepository _repository = repository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync(int pageNumber = PageOne)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new OrderHistoryViewModel();
            }

            IEnumerable<OrderEntity> orders = await _repository.FindSpecificCountOrderedReadOnlyAsync<OrderEntity, DateTime?>(pageNumber, OrdersPerPage, true, o => o.DateCompleted, o => o.UserId == currentUser.Id);
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
                    TotalPages = (int)Math.Ceiling((double)totalResults / OrdersPerPage),
                },
            };
        }

        public async Task<CartViewModel> CreateViewCartViewModelAsync(OrderInformationDto? orderInformationDto = null)
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

            var orderInformation = new OrderInformationDto();

            if (orderInformationDto != null)
            {
                orderInformation.Country = orderInformationDto.Country;
                orderInformation.City = orderInformationDto.City;
                orderInformation.Street = orderInformationDto.Street;
                orderInformation.PostalCode = orderInformationDto.PostalCode;
                orderInformation.AdditionalInformation = orderInformationDto.AdditionalInformation;
            }

            return new CartViewModel()
            {
                CartId = userCart.Id,
                CartItems = userCart.CartItems.Select(ci => new CartItemDto()
                {
                    Id = ci.Id,
                    ProductName = ci.Product.Name,
                    ProductImageUrl = ci.Product.ImageUrl,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    ProductId = ci.ProductId,
                }),
                OrderInformation = orderInformation,
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
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new OrderViewModel(); ;
            }

            var order = await _repository.FindOneReadOnlyAsync<OrderEntity>(o => o.Id == orderId && o.UserId == currentUser.Id, query => query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product));

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
                    ProductId = oi.ProductId,
                }),
            };
        }

        public async Task<AddToCartResultDto> AddToCartAsync(CreateCartItemDto createCartItemDto)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return new AddToCartResultDto()
                {
                    IsAddToCartSuccessful = false,
                    AddToCartMessage = GenericErrorMessage,
                };
            }

            var currentUserCart = await _repository.FindOneAsync<CartEntity>(ce => ce.UserID == currentUser.Id, query => query.Include(c => c.CartItems));

            if (currentUserCart == null)
            {
                return new AddToCartResultDto()
                {
                    IsAddToCartSuccessful = false,
                    AddToCartMessage = GenericErrorMessage,
                };
            }

            if (currentUserCart.CartItems.Any(ci => ci.ProductId == createCartItemDto.ProductId))
            {
                CartItemEntity cartItemToUpdate = currentUserCart.CartItems.First(ci => ci.ProductId == createCartItemDto.ProductId);
                cartItemToUpdate.Quantity += createCartItemDto.Quantity;
            }
            else
            {
                var cartItemToAdd = new CartItemEntity()
                {
                    Quantity = createCartItemDto.Quantity,
                    ProductId = createCartItemDto.ProductId,
                    CartId = currentUserCart.Id,
                };

                currentUserCart.CartItems.Add(cartItemToAdd);
            }

            await _repository.SaveChangesAsync();

            return new AddToCartResultDto()
            {
                IsAddToCartSuccessful = true,
                AddToCartMessage = ProductAddedToCartSuccessfully,
            };
        }

        public async Task<bool> CreateOrderAsync(OrderInformationDto orderInformationDto)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser == null)
            {
                return false;
            }

            var currentUserCart = await _repository.FindOneAsync<CartEntity>(ce => ce.UserID == currentUser.Id, query => query.Include(c => c.CartItems).ThenInclude(ci => ci.Product));

            if (currentUserCart == null)
            {
                return false;
            }

            var order = new OrderEntity()
            {
                DateCreated = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Country = orderInformationDto.Country,
                City = orderInformationDto.City,
                Street = orderInformationDto.Street,
                PostalCode = orderInformationDto.PostalCode,
                AdditionalInformation = orderInformationDto.AdditionalInformation,
                UserId = currentUser.Id,
                OrderItems = currentUserCart.CartItems.Select(ci => new OrderItemEntity()
                {
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    ProductId = ci.ProductId,
                }).ToList(),
            };

            await _repository.AddAsync(order);
            _repository.RemoveRange(currentUserCart.CartItems);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItemToRemove = await _repository.FindByIdAsync<CartItemEntity>(cartItemId);

            if (cartItemToRemove != null)
            {
                _repository.Remove(cartItemToRemove);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
