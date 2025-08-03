using LeaguePlaza.Core.Features.Admin.Services;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using Moq;
using System.Linq.Expressions;

namespace LeaguePlaza.Tests.Features.Admin
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private AdminService _adminService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _adminService = new AdminService(_mockRepository.Object);
        }

        [Test]
        [TestCase(1, 15, 10, 2)]
        [TestCase(2, 15, 10, 2)]
        [TestCase(1, 10, 10, 1)]
        [TestCase(1, 0, 10, 0)]
        public async Task CreateMountAdminViewModelAsync_ReturnsCorrectViewModelWithPagination(int pageNumber, int totalMounts, int mountsPerPage, int expectedTotalPages)
        {
            // Arrange
            var allMounts = new List<MountEntity>();

            for (int i = 1; i <= totalMounts; i++)
            {
                allMounts.Add(new MountEntity
                {
                    Id = i,
                    Name = $"Mount {i}",
                    Description = (i % 2 == 0) ? null : $"Description {i}",
                    Created = DateTime.UtcNow.AddDays(-i),
                    RentPrice = 10m * i,
                    ImageUrl = $"mount{i}.jpg",
                    MountType = MountType.Flying,
                    Rating = 3.0 + (i * 0.1)
                });
            }

            var mountsForPage = allMounts.Skip((pageNumber - 1) * mountsPerPage).Take(mountsPerPage).ToList();

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber,
                mountsPerPage,
                false,
                It.IsAny<Expression<Func<MountEntity, int>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(mountsForPage);

            _mockRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<MountEntity, bool>>>()))
                           .ReturnsAsync(totalMounts);

            // Act
            var viewModel = await _adminService.CreateMountAdminViewModelAsync(pageNumber);

            // Assert
            Assert.That(viewModel, Is.Not.Null);

            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber, mountsPerPage, false,
                It.IsAny<Expression<Func<MountEntity, int>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            ), Times.Once);

            _mockRepository.Verify(r => r.GetCountAsync(
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            ), Times.Once);

            Assert.That(viewModel.Mounts, Is.Not.Null);
            Assert.That(viewModel.Mounts.Count(), Is.EqualTo(mountsForPage.Count));

            if (mountsForPage.Count != 0)
            {
                var firstEntity = mountsForPage.First();
                var firstDto = viewModel.Mounts.First();

                Assert.Multiple(() =>
                {
                    Assert.That(firstDto.Id, Is.EqualTo(firstEntity.Id));
                    Assert.That(firstDto.Name, Is.EqualTo(firstEntity.Name));
                    Assert.That(firstDto.Description, Is.EqualTo(firstEntity.Description ?? "No description available"));
                    Assert.That(firstDto.RentPrice, Is.EqualTo(firstEntity.RentPrice));
                    Assert.That(firstDto.ImageUrl, Is.EqualTo(firstEntity.ImageUrl));
                    Assert.That(firstDto.Type, Is.EqualTo(firstEntity.MountType.ToString()));
                    Assert.That(firstDto.Rating, Is.EqualTo(firstEntity.Rating));
                });
            }


            Assert.That(viewModel.Pagination, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(pageNumber));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(expectedTotalPages));
            });
        }

        [Test]
        [TestCase(1, 15, 10, 2)]
        [TestCase(2, 15, 10, 2)]
        [TestCase(1, 10, 10, 1)]
        [TestCase(1, 0, 10, 0)]
        public async Task CreateProductAdminViewModelAsync_ReturnsCorrectViewModelWithPagination(int pageNumber, int totalProducts, int productsPerPage, int expectedTotalPages)
        {
            // Arrange
            var allProducts = new List<ProductEntity>();
            for (int i = 1; i <= totalProducts; i++)
            {
                allProducts.Add(new ProductEntity
                {
                    Id = i,
                    Name = $"Product {i}",
                    Description = (i % 2 == 0) ? null : $"Description {i}",
                    Price = 100m + (i * 5),
                    ImageUrl = $"product{i}.jpg",
                    IsInStock = (i % 2 == 1),
                    ProductType = ProductType.Enhancement,
                });
            }

            var productsForPage = allProducts.Skip((pageNumber - 1) * productsPerPage).Take(productsPerPage).ToList();

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber,
                productsPerPage,
                false,
                It.IsAny<Expression<Func<ProductEntity, int>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(productsForPage);

            _mockRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<ProductEntity, bool>>>()))
                           .ReturnsAsync(totalProducts);

            // Act
            var viewModel = await _adminService.CreateProductAdminViewModelAsync(pageNumber);

            // Assert
            Assert.That(viewModel, Is.Not.Null);

            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber, productsPerPage, false,
                It.IsAny<Expression<Func<ProductEntity, int>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            ), Times.Once);

            _mockRepository.Verify(r => r.GetCountAsync(
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            ), Times.Once);

            Assert.That(viewModel.Products, Is.Not.Null);
            Assert.That(viewModel.Products.Count(), Is.EqualTo(productsForPage.Count));

            if (productsForPage.Count != 0)
            {
                var firstEntity = productsForPage.First();
                var firstDto = viewModel.Products.First();

                Assert.Multiple(() =>
                {
                    Assert.That(firstDto.Id, Is.EqualTo(firstEntity.Id));
                    Assert.That(firstDto.Name, Is.EqualTo(firstEntity.Name));
                    Assert.That(firstDto.Description, Is.EqualTo(firstEntity.Description ?? "No description available"));
                    Assert.That(firstDto.Price, Is.EqualTo(firstEntity.Price));
                    Assert.That(firstDto.ImageUrl, Is.EqualTo(firstEntity.ImageUrl));
                    Assert.That(firstDto.IsInStock, Is.EqualTo(firstEntity.IsInStock));
                    Assert.That(firstDto.ProductType, Is.EqualTo(firstEntity.ProductType.ToString()));
                });
            }


            Assert.That(viewModel.Pagination, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(pageNumber));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(expectedTotalPages));
            });
        }

        [Test]
        [TestCase(1, 15, 10, 2)]
        [TestCase(2, 15, 10, 2)]
        [TestCase(1, 10, 10, 1)]
        [TestCase(1, 0, 10, 0)]
        public async Task CreateOrderAdminViewModelAsync_ReturnsCorrectViewModelWithPagination(int pageNumber, int totalOrders, int ordersPerPage, int expectedTotalPages)
        {
            // Arrange
            var now = DateTime.UtcNow;
            var allOrders = new List<OrderEntity>();
            for (int i = 1; i <= totalOrders; i++)
            {
                allOrders.Add(new OrderEntity
                {
                    Id = i,
                    DateCreated = now.AddDays(-i),
                    DateCompleted = (i % 3 == 0) ? null : now.AddDays(-i + 1),
                    Status = (i % 2 == 0) ? OrderStatus.Completed  : OrderStatus.Pending
                });
            }

            var ordersForPage = allOrders.Skip((pageNumber - 1) * ordersPerPage).Take(ordersPerPage).ToList();

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber,
                ordersPerPage,
                false,
                It.IsAny<Expression<Func<OrderEntity, int>>>(),
                It.IsAny<Expression<Func<OrderEntity, bool>>>()
            )).ReturnsAsync(ordersForPage);

            _mockRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<OrderEntity, bool>>>()))
                           .ReturnsAsync(totalOrders);

            // Act
            var viewModel = await _adminService.CreateOrderAdminViewModelAsync(pageNumber);

            // Assert
            Assert.That(viewModel, Is.Not.Null);

            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber, ordersPerPage, false,
                It.IsAny<Expression<Func<OrderEntity, int>>>(),
                It.IsAny<Expression<Func<OrderEntity, bool>>>()
            ), Times.Once);

            _mockRepository.Verify(r => r.GetCountAsync(
                It.IsAny<Expression<Func<OrderEntity, bool>>>()
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
    }
}
