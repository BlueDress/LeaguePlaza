using LeaguePlaza.Core.Features.Product.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Product.Services;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using Moq;
using System.Linq.Expressions;

namespace LeaguePlaza.Tests.Features.Product
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDropboxService> _mockDropboxService;
        private ProductService _productService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _mockDropboxService = new Mock<IDropboxService>();
            _productService = new ProductService(_mockRepository.Object, _mockDropboxService.Object);
        }

        [Test]
        public async Task CreateAvailableProductsViewModelAsync_ReturnsCorrectViewModelWithAvailableProducts()
        {
            // Arrange
            var availableProducts = new List<ProductEntity>
            {
                new ProductEntity { Id = 1, Name = "Potion 1", Description = "Test", Price = 10m, ImageUrl = "potion1.jpg", IsInStock = true, ProductType = ProductType.Healing },
                new ProductEntity { Id = 2, Name = "Potion 2", Description = "", Price = 20m, ImageUrl = "potion2.jpg", IsInStock = true, ProductType = ProductType.Enhancement },
            };
            int totalResults = 2;

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, false, It.IsAny<Expression<Func<ProductEntity, string>>>(), It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(availableProducts);

            _mockRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(totalResults);

            // Act
            var viewModel = await _productService.CreateAvailableProductsViewModelAsync();

            // Assert
            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, false,
                It.IsAny<Expression<Func<ProductEntity, string>>>(),
                It.Is<Expression<Func<ProductEntity, bool>>>(expr => expr.Compile()(new ProductEntity { IsInStock = true }))
            ), Times.Once);

            _mockRepository.Verify(r => r.GetCountAsync(
                It.Is<Expression<Func<ProductEntity, bool>>>(expr => expr.Compile()(new ProductEntity { IsInStock = true }))
            ), Times.Once);

            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Products, Is.Not.Null);
            Assert.That(viewModel.Products.Count(), Is.EqualTo(2));

            var firstProductDto = viewModel.Products.First();

            Assert.Multiple(() =>
            {
                Assert.That(firstProductDto.Id, Is.EqualTo(1));
                Assert.That(firstProductDto.Name, Is.EqualTo("Potion 1"));
                Assert.That(firstProductDto.Description, Is.EqualTo("Test"));
                Assert.That(firstProductDto.Price, Is.EqualTo(10m));
                Assert.That(firstProductDto.ImageUrl, Is.EqualTo("potion1.jpg"));
                Assert.That(firstProductDto.IsInStock, Is.True);
                Assert.That(firstProductDto.ProductType, Is.EqualTo(ProductType.Healing.ToString()));
            });

            var secondProductDto = viewModel.Products.Last();

            Assert.Multiple(() =>
            {
                Assert.That(secondProductDto.Description, Is.EqualTo("No description available"));

                Assert.That(viewModel.Pagination, Is.Not.Null);
            });

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(1));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task CreateAvailableProductsViewModelAsync_NoProductsInStock_ReturnsEmptyViewModel()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<Expression<Func<ProductEntity, string>>>(), It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(new List<ProductEntity>());

            _mockRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(0);

            // Act
            var viewModel = await _productService.CreateAvailableProductsViewModelAsync();

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Products, Is.Empty);
                Assert.That(viewModel.Pagination, Is.Not.Null);
            });

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(1));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task CreateViewProductViewModelAsync_ProductFound_ReturnsCorrectViewModelWithRecommendations()
        {
            // Arrange
            int productId = 1;
            var mainProduct = new ProductEntity
            {
                Id = productId,
                Name = "Main Product",
                Description = "A detailed description.",
                Price = 100m,
                ImageUrl = "main.jpg",
                IsInStock = true,
                ProductType = ProductType.Enhancement
            };

            var recommendedProduct1 = new ProductEntity { Id = 2, Name = "Rec Product 1", Description = "Rec Desc 1", Price = 50m, ImageUrl = "rec1.jpg", IsInStock = true, ProductType = ProductType.Enhancement };
            var recommendedProduct2 = new ProductEntity { Id = 3, Name = "Rec Product 2", Description = "Rec Desc 2", Price = 75m, ImageUrl = "rec2.jpg", IsInStock = true, ProductType = ProductType.Enhancement };

            _mockRepository.Setup(r => r.FindByIdAsync<ProductEntity>(productId)).ReturnsAsync(mainProduct);
            _mockRepository.Setup(r => r.FindSpecificCountReadOnlyAsync(
                3, It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(new List<ProductEntity> { recommendedProduct1, recommendedProduct2 });

            // Act
            var viewModel = await _productService.CreateViewProductViewModelAsync(productId);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<ProductEntity>(productId), Times.Once);
            _mockRepository.Verify(r => r.FindSpecificCountReadOnlyAsync(
                3,
                It.Is<Expression<Func<ProductEntity, bool>>>(expr =>
                    expr.Compile()(new ProductEntity { Id = productId + 1, ProductType = mainProduct.ProductType }) &&
                    !expr.Compile()(new ProductEntity { Id = productId, ProductType = mainProduct.ProductType })
                )
            ), Times.Once);

            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.Product, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Product.Id, Is.EqualTo(mainProduct.Id));
                Assert.That(viewModel.Product.Name, Is.EqualTo(mainProduct.Name));
                Assert.That(viewModel.Product.Description, Is.EqualTo(mainProduct.Description));
                Assert.That(viewModel.Product.Price, Is.EqualTo(mainProduct.Price));
                Assert.That(viewModel.Product.ImageUrl, Is.EqualTo(mainProduct.ImageUrl));
                Assert.That(viewModel.Product.IsInStock, Is.True);
                Assert.That(viewModel.Product.ProductType, Is.EqualTo(mainProduct.ProductType.ToString()));

                Assert.That(viewModel.RecommendedProducts, Is.Not.Null);
            });

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.RecommendedProducts.Count(), Is.EqualTo(2));
                Assert.That(viewModel.RecommendedProducts.First().Id, Is.EqualTo(recommendedProduct1.Id));
                Assert.That(viewModel.RecommendedProducts.Last().Id, Is.EqualTo(recommendedProduct2.Id));
            });
        }

        [Test]
        public async Task UpdateProductAsync_ProductNotFound_DoesNothing()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto { Id = 999, Name = "NonExistent", Price = 1m, ProductType = ProductType.Healing.ToString() };
            _mockRepository.Setup(r => r.FindByIdAsync<ProductEntity>(updateProductDto.Id)).ReturnsAsync((ProductEntity)null!);

            // Act
            await _productService.UpdateProductAsync(updateProductDto);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<ProductEntity>(updateProductDto.Id), Times.Once);
            _mockRepository.Verify(r => r.Update(It.IsAny<ProductEntity>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            _mockDropboxService.Verify(d => d.GetAccessToken(), Times.Never);
        }

        [Test]
        public async Task DeleteProductAsync_ProductFound_RemovesProductAndSavesChanges()
        {
            // Arrange
            int productIdToRemove = 123;
            var product = new ProductEntity { Id = productIdToRemove, Name = "To Be Deleted" };
            _mockRepository.Setup(r => r.FindByIdAsync<ProductEntity>(productIdToRemove)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.Remove(It.IsAny<ProductEntity>())).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _productService.DeleteProductAsync(productIdToRemove);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<ProductEntity>(productIdToRemove), Times.Once);
            _mockRepository.Verify(r => r.Remove(product), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteProductAsync_ProductNotFound_DoesNothing()
        {
            // Arrange
            int productIdToRemove = 999;
            _mockRepository.Setup(r => r.FindByIdAsync<ProductEntity>(productIdToRemove)).ReturnsAsync((ProductEntity)null!);
            _mockRepository.Setup(r => r.Remove(It.IsAny<ProductEntity>())).Verifiable();

            // Act
            await _productService.DeleteProductAsync(productIdToRemove);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<ProductEntity>(productIdToRemove), Times.Once);
            _mockRepository.Verify(r => r.Remove(It.IsAny<ProductEntity>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
