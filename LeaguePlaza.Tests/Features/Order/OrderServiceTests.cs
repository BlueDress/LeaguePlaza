using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Services;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace LeaguePlaza.Tests.Features.Order
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private OrderService _orderService;
        private const string TEST_USER_ID = "testUserId123";

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _orderService = new OrderService(_mockRepository.Object);
        }

        [Test]
        [TestCase(1, 15, 10, 2)]
        [TestCase(2, 15, 10, 2)]
        [TestCase(1, 10, 10, 1)]
        [TestCase(1, 0, 10, 0)]
        public async Task CreateOrderHistoryViewModelAsync_ReturnsCorrectViewModelWithPagination(int pageNumber, int totalOrders, int ordersPerPage, int expectedTotalPages)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var allOrders = new List<OrderEntity>();

            for (int i = 1; i <= totalOrders; i++)
            {
                allOrders.Add(new OrderEntity
                {
                    Id = i,
                    UserId = TEST_USER_ID,
                    DateCreated = now.AddDays(-i),
                    DateCompleted = (i % 3 == 0) ? null : now.AddDays(-i + 1),
                    Status = (i % 2 == 0) ? OrderStatus.Completed : OrderStatus.Pending
                });
            }

            var ordersForPage = allOrders
                .OrderByDescending(o => o.DateCompleted ?? o.DateCreated)
                .Skip((pageNumber - 1) * ordersPerPage)
                .Take(ordersPerPage)
                .ToList();

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber,
                ordersPerPage,
                true,
                It.IsAny<Expression<Func<OrderEntity, DateTime?>>>(), It.IsAny<Expression<Func<OrderEntity, bool>>>()
            )).ReturnsAsync(ordersForPage);

            _mockRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<OrderEntity, bool>>>()
            )).ReturnsAsync(totalOrders);

            // Act
            var viewModel = await _orderService.CreateOrderHistoryViewModelAsync(TEST_USER_ID, pageNumber);

            // Assert
            Assert.That(viewModel, Is.Not.Null);

            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber, ordersPerPage, true,
                It.IsAny<Expression<Func<OrderEntity, DateTime?>>>(),
                It.Is<Expression<Func<OrderEntity, bool>>>(expr => expr.Compile()(new OrderEntity { UserId = TEST_USER_ID }))
            ), Times.Once);

            _mockRepository.Verify(r => r.GetCountAsync(
                It.Is<Expression<Func<OrderEntity, bool>>>(expr => expr.Compile()(new OrderEntity { UserId = TEST_USER_ID }))
            ), Times.Once);

            Assert.That(viewModel.Orders, Is.Not.Null);
            Assert.That(viewModel.Orders.Count(), Is.EqualTo(ordersForPage.Count));

            if (ordersForPage.Count != 0)
            {
                for (int i = 0; i < ordersForPage.Count; i++)
                {
                    var entity = ordersForPage[i];
                    var dto = viewModel.Orders.ElementAt(i);

                    Assert.Multiple(() =>
                    {
                        Assert.That(dto.Id, Is.EqualTo(entity.Id));
                        Assert.That(dto.DateCreated, Is.EqualTo(entity.DateCreated.ToString("dd.MM.yyyy")));
                        Assert.That(dto.DateCompleted, Is.EqualTo(entity.DateCompleted.HasValue ? entity.DateCompleted.Value.ToString("dd.MM.yyyy") : string.Empty));
                        Assert.That(dto.Status, Is.EqualTo(entity.Status.ToString()));
                    });
                }
            }


            Assert.That(viewModel.Pagination, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(pageNumber));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(expectedTotalPages));
            });
        }

        [Test]
        public async Task CreateViewCartViewModelAsync_UserHasNoCart_CreatesNewCartAndReturnsEmptyViewModel()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync((CartEntity)null!);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<CartEntity>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var viewModel = await _orderService.CreateViewCartViewModelAsync(TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.FindOneReadOnlyAsync(
                It.Is<Expression<Func<CartEntity, bool>>>(expr => expr.Compile()(new CartEntity { UserID = TEST_USER_ID })),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            ), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.Is<CartEntity>(c => c.UserID == TEST_USER_ID)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.CartItems, Is.Empty);
        }

        [Test]
        public async Task CreateViewCartViewModelAsync_UserHasCartWithItems_ReturnsCorrectViewModel()
        {
            // Arrange
            var product1 = new ProductEntity { Id = 101, Name = "Potion 1", ImageUrl = "potion1.jpg", Price = 50.00m };
            var product2 = new ProductEntity { Id = 102, Name = "Potion 2", ImageUrl = "potion2.png", Price = 5.00m };

            var cartItems = new List<CartItemEntity>
            {
                new CartItemEntity { Id = 1, ProductId = product1.Id, Product = product1, Quantity = 2 },
                new CartItemEntity { Id = 2, ProductId = product2.Id, Product = product2, Quantity = 5 },
            };

            var userCart = new CartEntity { Id = 10, UserID = TEST_USER_ID, CartItems = cartItems };

            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync(userCart);

            // Act
            var viewModel = await _orderService.CreateViewCartViewModelAsync(TEST_USER_ID);

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.CartId, Is.EqualTo(userCart.Id));
                Assert.That(viewModel.CartItems, Is.Not.Null);
            });
            Assert.That(viewModel.CartItems.Count(), Is.EqualTo(cartItems.Count));

            var firstCartItemDto = viewModel.CartItems.First();
            Assert.Multiple(() =>
            {
                Assert.That(firstCartItemDto.Id, Is.EqualTo(cartItems[0].Id));
                Assert.That(firstCartItemDto.ProductName, Is.EqualTo(cartItems[0].Product.Name));
                Assert.That(firstCartItemDto.ProductImageUrl, Is.EqualTo(cartItems[0].Product.ImageUrl));
                Assert.That(firstCartItemDto.Quantity, Is.EqualTo(cartItems[0].Quantity));
                Assert.That(firstCartItemDto.Price, Is.EqualTo(cartItems[0].Product.Price));
                Assert.That(firstCartItemDto.ProductId, Is.EqualTo(cartItems[0].ProductId));

                Assert.That(viewModel.OrderInformation, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.OrderInformation.Country, Is.Null.Or.Empty);
                Assert.That(viewModel.OrderInformation.City, Is.Null.Or.Empty);
            });
        }

        [Test]
        public async Task CreateViewCartViewModelAsync_UserHasCartAndOrderInformationDtoProvided_PopulatesOrderInformation()
        {
            // Arrange
            var userCart = new CartEntity { Id = 10, UserID = TEST_USER_ID, CartItems = new List<CartItemEntity>() };
            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync(userCart);

            var providedOrderInfo = new OrderInformationDto
            {
                Country = "Bulgaria",
                City = "Sofia",
                Street = "Vitosha Blvd",
                PostalCode = "1000",
                AdditionalInformation = "Near the lion bridge"
            };

            // Act
            var viewModel = await _orderService.CreateViewCartViewModelAsync(TEST_USER_ID, providedOrderInfo);

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.OrderInformation, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.OrderInformation.Country, Is.EqualTo(providedOrderInfo.Country));
                Assert.That(viewModel.OrderInformation.City, Is.EqualTo(providedOrderInfo.City));
                Assert.That(viewModel.OrderInformation.Street, Is.EqualTo(providedOrderInfo.Street));
                Assert.That(viewModel.OrderInformation.PostalCode, Is.EqualTo(providedOrderInfo.PostalCode));
                Assert.That(viewModel.OrderInformation.AdditionalInformation, Is.EqualTo(providedOrderInfo.AdditionalInformation));
            });
        }

        [Test]
        public async Task GetCartItemsCountAsync_UserHasNoCart_CreatesNewCartAndReturnsZero()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>()
            )).ReturnsAsync((CartEntity)null!);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<CartEntity>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var count = await _orderService.GetCartItemsCountAsync(TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.FindOneReadOnlyAsync(
                It.Is<Expression<Func<CartEntity, bool>>>(expr => expr.Compile()(new CartEntity { UserID = TEST_USER_ID }))
            ), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.Is<CartEntity>(c => c.UserID == TEST_USER_ID)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockRepository.Verify(r => r.GetCountAsync(It.IsAny<Expression<Func<CartItemEntity, bool>>>()), Times.Never);

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetCartItemsCountAsync_UserHasCartWithItems_ReturnsCorrectCount()
        {
            // Arrange
            var userCart = new CartEntity { Id = 10, UserID = TEST_USER_ID };

            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>()
            )).ReturnsAsync(userCart);

            _mockRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<CartItemEntity, bool>>>()
            )).ReturnsAsync(5);

            // Act
            var count = await _orderService.GetCartItemsCountAsync(TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.FindOneReadOnlyAsync(
                It.Is<Expression<Func<CartEntity, bool>>>(expr => expr.Compile()(new CartEntity { UserID = TEST_USER_ID }))
            ), Times.Once);
            _mockRepository.Verify(r => r.GetCountAsync(
                It.Is<Expression<Func<CartItemEntity, bool>>>(expr => expr.Compile()(new CartItemEntity { CartId = userCart.Id }))
            ), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<CartEntity>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);

            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public async Task CreateOrderViewModelAsync_OrderNotFoundOrNotBelongingToUser_ReturnsEmptyViewModel()
        {
            // Arrange
            int orderId = 99;

            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<OrderEntity, bool>>>(),
                It.IsAny<Func<IQueryable<OrderEntity>, IIncludableQueryable<OrderEntity, object>>>()
            )).ReturnsAsync((OrderEntity)null!);

            // Act
            var viewModel = await _orderService.CreateOrderViewModelAsync(orderId, TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.FindOneReadOnlyAsync(
                It.Is<Expression<Func<OrderEntity, bool>>>(expr => expr.Compile()(new OrderEntity { Id = orderId, UserId = TEST_USER_ID })),
                It.IsAny<Func<IQueryable<OrderEntity>, IIncludableQueryable<OrderEntity, object>>>()
            ), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.OrderItems, Is.Empty);
                Assert.That(viewModel, Is.Not.Null);
            });
        }

        [Test]
        public async Task CreateOrderViewModelAsync_OrderFoundWithItems_ReturnsCorrectViewModel()
        {
            // Arrange
            int orderId = 1;
            var product1 = new ProductEntity { Id = 101, Name = "Potion 1", ImageUrl = "potion1.jpg", Price = 50.00m };
            var product2 = new ProductEntity { Id = 102, Name = "Potion 2", ImageUrl = "potion2.png", Price = 5.00m };

            var orderItems = new List<OrderItemEntity>
            {
                new OrderItemEntity { Id = 10, ProductId = product1.Id, Product = product1, Quantity = 1, Price = product1.Price },
                new OrderItemEntity { Id = 11, ProductId = product2.Id, Product = product2, Quantity = 2, Price = product2.Price },
            };

            var orderEntity = new OrderEntity
            {
                Id = orderId,
                UserId = TEST_USER_ID,
                DateCreated = DateTime.UtcNow.AddDays(-5),
                DateCompleted = DateTime.UtcNow.AddDays(-1),
                Status = OrderStatus.Completed,
                Country = "Test Country",
                City = "Test City",
                Street = "Test Street",
                PostalCode = "12345",
                OrderItems = orderItems
            };

            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<OrderEntity, bool>>>(),
                It.IsAny<Func<IQueryable<OrderEntity>, IIncludableQueryable<OrderEntity, object>>>()
            )).ReturnsAsync(orderEntity);

            // Act
            var viewModel = await _orderService.CreateOrderViewModelAsync(orderId, TEST_USER_ID);

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Order, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Order.Id, Is.EqualTo(orderEntity.Id));
                Assert.That(viewModel.Order.DateCreated, Is.EqualTo(orderEntity.DateCreated.ToString("dd.MM.yyyy")));
                Assert.That(viewModel.Order.DateCompleted, Is.EqualTo(orderEntity.DateCompleted!.Value.ToString("dd.MM.yyyy")));
                Assert.That(viewModel.Order.Status, Is.EqualTo(orderEntity.Status.ToString()));

                Assert.That(viewModel.OrderItems, Is.Not.Null);
            });
            Assert.That(viewModel.OrderItems.Count(), Is.EqualTo(orderItems.Count));

            var firstOrderItemDto = viewModel.OrderItems.First();
            Assert.Multiple(() =>
            {
                Assert.That(firstOrderItemDto.ProductName, Is.EqualTo(orderItems[0].Product.Name));
                Assert.That(firstOrderItemDto.ProductImageUrl, Is.EqualTo(orderItems[0].Product.ImageUrl));
                Assert.That(firstOrderItemDto.Quantity, Is.EqualTo(orderItems[0].Quantity));
                Assert.That(firstOrderItemDto.Price, Is.EqualTo(orderItems[0].Price));
                Assert.That(firstOrderItemDto.ProductId, Is.EqualTo(orderItems[0].ProductId));
            });
        }

        [Test]
        public async Task CreateOrderViewModelAsync_OrderFoundWithNullDateCompleted_MapsDateCompletedToNull()
        {
            // Arrange
            int orderId = 2;
            var orderEntity = new OrderEntity
            {
                Id = orderId,
                UserId = TEST_USER_ID,
                DateCreated = DateTime.UtcNow.AddDays(-2),
                DateCompleted = null,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItemEntity>()
            };

            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<OrderEntity, bool>>>(),
                It.IsAny<Func<IQueryable<OrderEntity>, IIncludableQueryable<OrderEntity, object>>>()
            )).ReturnsAsync(orderEntity);

            // Act
            var viewModel = await _orderService.CreateOrderViewModelAsync(orderId, TEST_USER_ID);

            // Assert
            Assert.That(viewModel.Order.DateCompleted, Is.Null);
        }

        [Test]
        public async Task AddToCartAsync_UserCartNotFound_ReturnsFailure()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindOneAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync((CartEntity)null!);

            var createCartItemDto = new CreateCartItemDto { ProductId = 1, Quantity = 1 };

            // Act
            var result = await _orderService.AddToCartAsync(createCartItemDto, TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsAddToCartSuccessful, Is.False);
                Assert.That(result.AddToCartMessage, Is.EqualTo("Something went wrong"));
            });
        }

        [Test]
        public async Task AddToCartAsync_ProductAlreadyInCart_UpdatesQuantity()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Existing Product", Price = 10m };
            var existingCartItem = new CartItemEntity { Id = 101, ProductId = product.Id, Product = product, Quantity = 2 };
            var currentUserCart = new CartEntity { Id = 1, UserID = TEST_USER_ID, CartItems = new List<CartItemEntity> { existingCartItem } };

            _mockRepository.Setup(r => r.FindOneAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync(currentUserCart);

            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var createCartItemDto = new CreateCartItemDto { ProductId = 1, Quantity = 3 };

            // Act
            var result = await _orderService.AddToCartAsync(createCartItemDto, TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<CartItemEntity>()), Times.Never);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsAddToCartSuccessful, Is.True);
                Assert.That(result.AddToCartMessage, Is.EqualTo("Product added to cart"));
                Assert.That(existingCartItem.Quantity, Is.EqualTo(5));
            });
        }

        [Test]
        public async Task AddToCartAsync_ProductNotInCart_AddsNewCartItem()
        {
            // Arrange
            var currentUserCart = new CartEntity { Id = 1, UserID = TEST_USER_ID, CartItems = new List<CartItemEntity>() };

            _mockRepository.Setup(r => r.FindOneAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync(currentUserCart);

            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var createCartItemDto = new CreateCartItemDto { ProductId = 2, Quantity = 1 };

            // Act
            var result = await _orderService.AddToCartAsync(createCartItemDto, TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<CartItemEntity>()), Times.Never);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsAddToCartSuccessful, Is.True);
                Assert.That(result.AddToCartMessage, Is.EqualTo("Product added to cart"));
                Assert.That(currentUserCart.CartItems.Count, Is.EqualTo(1));
            });
            Assert.Multiple(() =>
            {
                Assert.That(currentUserCart.CartItems.First().ProductId, Is.EqualTo(createCartItemDto.ProductId));
                Assert.That(currentUserCart.CartItems.First().Quantity, Is.EqualTo(createCartItemDto.Quantity));
            });
        }

        [Test]
        public async Task CreateOrderAsync_UserCartNotFound_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindOneAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync((CartEntity)null!);

            var orderInfo = new OrderInformationDto();

            // Act
            var result = await _orderService.CreateOrderAsync(orderInfo, TEST_USER_ID);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<OrderEntity>()), Times.Never);
            _mockRepository.Verify(r => r.RemoveRange(It.IsAny<ICollection<CartItemEntity>>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateOrderAsync_UserCartFoundWithItems_CreatesOrderClearsCartAndReturnsTrue()
        {
            // Arrange
            var product1 = new ProductEntity { Id = 101, Name = "Potion 1", ImageUrl = "potion1.jpg", Price = 50.00m };
            var product2 = new ProductEntity { Id = 102, Name = "Potion 2", ImageUrl = "potion2.jpg", Price = 5.00m };

            var cartItems = new List<CartItemEntity>
            {
                new CartItemEntity { Id = 1, ProductId = product1.Id, Product = product1, Quantity = 2 },
                new CartItemEntity { Id = 2, ProductId = product2.Id, Product = product2, Quantity = 1 },
            };

            var currentUserCart = new CartEntity { Id = 1, UserID = TEST_USER_ID, CartItems = cartItems };

            _mockRepository.Setup(r => r.FindOneAsync(
                It.IsAny<Expression<Func<CartEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CartEntity>, IIncludableQueryable<CartEntity, object>>>()
            )).ReturnsAsync(currentUserCart);

            OrderEntity capturedOrder = null!;
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<OrderEntity>()))
                           .Callback<OrderEntity>(order => capturedOrder = order)
                           .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.RemoveRange(It.IsAny<ICollection<CartItemEntity>>())).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var orderInfo = new OrderInformationDto
            {
                Country = "TestCountry",
                City = "TestCity",
                Street = "TestStreet",
                PostalCode = "TestPostal",
                AdditionalInformation = "Test Info"
            };

            // Act
            var result = await _orderService.CreateOrderAsync(orderInfo, TEST_USER_ID);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<OrderEntity>()), Times.Once);
            _mockRepository.Verify(r => r.RemoveRange(currentUserCart.CartItems), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.That(capturedOrder, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(capturedOrder.UserId, Is.EqualTo(TEST_USER_ID));
                Assert.That(capturedOrder.Status, Is.EqualTo(OrderStatus.Pending));
                Assert.That(capturedOrder.Country, Is.EqualTo(orderInfo.Country));
                Assert.That(capturedOrder.City, Is.EqualTo(orderInfo.City));
                Assert.That(capturedOrder.Street, Is.EqualTo(orderInfo.Street));
                Assert.That(capturedOrder.PostalCode, Is.EqualTo(orderInfo.PostalCode));
                Assert.That(capturedOrder.AdditionalInformation, Is.EqualTo(orderInfo.AdditionalInformation));
                Assert.That(capturedOrder.OrderItems.Count, Is.EqualTo(2));
            });

            var firstOrderItem = capturedOrder.OrderItems.First();
            Assert.Multiple(() =>
            {
                Assert.That(firstOrderItem.ProductId, Is.EqualTo(product1.Id));
                Assert.That(firstOrderItem.Quantity, Is.EqualTo(2));
                Assert.That(firstOrderItem.Price, Is.EqualTo(product1.Price));
            });

            var secondOrderItem = capturedOrder.OrderItems.Last();
            Assert.Multiple(() =>
            {
                Assert.That(secondOrderItem.ProductId, Is.EqualTo(product2.Id));
                Assert.That(secondOrderItem.Quantity, Is.EqualTo(1));
                Assert.That(secondOrderItem.Price, Is.EqualTo(product2.Price));
            });
        }

        [Test]
        public async Task RemoveCartItemAsync_CartItemFound_RemovesItemAndSavesChanges()
        {
            // Arrange
            int cartItemIdToRemove = 123;
            var cartItem = new CartItemEntity { Id = cartItemIdToRemove, ProductId = 1, Quantity = 1 };
            _mockRepository.Setup(r => r.FindByIdAsync<CartItemEntity>(cartItemIdToRemove)).ReturnsAsync(cartItem);
            _mockRepository.Setup(r => r.Remove(It.IsAny<CartItemEntity>())).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _orderService.RemoveCartItemAsync(cartItemIdToRemove);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<CartItemEntity>(cartItemIdToRemove), Times.Once);
            _mockRepository.Verify(r => r.Remove(cartItem), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveCartItemAsync_CartItemNotFound_DoesNothing()
        {
            // Arrange
            int cartItemIdToRemove = 999;
            _mockRepository.Setup(r => r.FindByIdAsync<CartItemEntity>(cartItemIdToRemove)).ReturnsAsync((CartItemEntity)null!);
            _mockRepository.Setup(r => r.Remove(It.IsAny<CartItemEntity>())).Verifiable();

            // Act
            await _orderService.RemoveCartItemAsync(cartItemIdToRemove);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<CartItemEntity>(cartItemIdToRemove), Times.Once);
            _mockRepository.Verify(r => r.Remove(It.IsAny<CartItemEntity>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task UpdateOrderStatusAsync_OrderNotFound_DoesNothing()
        {
            // Arrange
            int orderId = 999;
            var updateDto = new UpdateOrderStatusDto { Id = orderId, Status = OrderStatus.Completed.ToString() };
            _mockRepository.Setup(r => r.FindByIdAsync<OrderEntity>(orderId)).ReturnsAsync((OrderEntity)null!);

            // Act
            await _orderService.UpdateOrderStatusAsync(updateDto);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<OrderEntity>(orderId), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task UpdateOrderStatusAsync_StatusToCompleted_UpdatesStatusAndSetsDateCompleted()
        {
            // Arrange
            int orderId = 1;
            var orderToUpdate = new OrderEntity { Id = orderId, Status = OrderStatus.Pending, DateCompleted = null };
            var updateDto = new UpdateOrderStatusDto { Id = orderId, Status = OrderStatus.Completed.ToString() };

            _mockRepository.Setup(r => r.FindByIdAsync<OrderEntity>(orderId)).ReturnsAsync(orderToUpdate);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _orderService.UpdateOrderStatusAsync(updateDto);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<OrderEntity>(orderId), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(orderToUpdate.Status, Is.EqualTo(OrderStatus.Completed));
                Assert.That(orderToUpdate.DateCompleted, Is.Not.Null);
            });
            Assert.That(orderToUpdate.DateCompleted!.Value, Is.InRange(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(5)));
        }

        [Test]
        public async Task UpdateOrderStatusAsync_StatusToNonCompleted_UpdatesStatusAndSetsDateCompletedToNull()
        {
            // Arrange
            int orderId = 1;
            // Start with a completed order to test setting DateCompleted to null
            var orderToUpdate = new OrderEntity { Id = orderId, Status = OrderStatus.Completed, DateCompleted = DateTime.UtcNow.AddDays(-1) };
            var updateDto = new UpdateOrderStatusDto { Id = orderId, Status = OrderStatus.InProgress.ToString() };

            _mockRepository.Setup(r => r.FindByIdAsync<OrderEntity>(orderId)).ReturnsAsync(orderToUpdate);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _orderService.UpdateOrderStatusAsync(updateDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(orderToUpdate.Status, Is.EqualTo(OrderStatus.InProgress));
                Assert.That(orderToUpdate.DateCompleted, Is.Null);
            });
            _mockRepository.Verify(r => r.FindByIdAsync<OrderEntity>(orderId), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
