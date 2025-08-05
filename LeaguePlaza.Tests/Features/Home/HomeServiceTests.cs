using LeaguePlaza.Core.Features.Home.Services;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using Moq;
using System.Linq.Expressions;

namespace LeaguePlaza.Tests.Features.Home
{
    [TestFixture]
    public class HomeServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private HomeService _homeService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _homeService = new HomeService(_mockRepository.Object);
        }

        [Test]
        public async Task CreateHomePageViewModelAsync_RetrievesAllDataAndMapsCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var quests = new List<QuestEntity>
            {
                new() { Id = 1, Title = "Quest 1", Description = "Desc 1", Created = now.AddDays(-1), RewardAmount = 100, Type = QuestType.MonsterHunt, ImageName = "quest1.jpg", },
                new() { Id = 2, Title = "Quest 2", Description = null, Created = now.AddDays(-2), RewardAmount = 200, Type = QuestType.Gathering, ImageName = "quest2.jpg", },
                new() { Id = 3, Title = "Quest 3", Description = "Desc 3", Created = now.AddDays(-3), RewardAmount = 300, Type = QuestType.Escort, ImageName = "quest3.jpg", },
            };

            var mounts = new List<MountEntity>
            {
                new() { Id = 1, Name = "Mount 1", Description = "Mount Desc 1", RentPrice = 50, ImageUrl = "mount1.jpg", MountType = MountType.Flying, Rating = 4.5 },
                new() { Id = 2, Name = "Mount 2", Description = null, RentPrice = 60, ImageUrl = "mount2.jpg", MountType = MountType.Aquatic, Rating = 3.0 },
                new() { Id = 3, Name = "Mount 3", Description = "Mount Desc 3", RentPrice = 70, ImageUrl = "mount3.jpg", MountType = MountType.Ground, Rating = 5.0 },
            };

            var products = new List<ProductEntity>
            {
                new() { Id = 1, Name = "Product 1", Description = "Prod Desc 1", Price = 10.50m, ImageUrl = "prod1.jpg", ProductType = ProductType.Enhancement, },
                new() { Id = 2, Name = "Product 2", Description = null, Price = 5.99m, ImageUrl = "prod2.jpg", ProductType = ProductType.Impairment, },
                new() { Id = 3, Name = "Product 3", Description = "Prod Desc 3", Price = 20.00m, ImageUrl = "prod3.jpg", ProductType = ProductType.Healing, },
            };

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1,
                3,
                true,
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(quests);

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1,
                3,
                true,
                It.IsAny<Expression<Func<MountEntity, DateTime>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(mounts);

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1,
                3,
                false,
                It.IsAny<Expression<Func<ProductEntity, decimal>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(products);

            // Act
            var viewModel = await _homeService.CreateHomePageViewModelAsync();

            // Assert
            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 3, true,
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            ), Times.Once);

            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 3, true,
                It.IsAny<Expression<Func<MountEntity, DateTime>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            ), Times.Once);

            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 3, false,
                It.IsAny<Expression<Func<ProductEntity, decimal>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            ), Times.Once);

            Assert.That(viewModel, Is.Not.Null);

            Assert.That(viewModel.LatestQuests, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.LatestQuests.Count(), Is.EqualTo(quests.Count));
                Assert.That(viewModel.LatestQuests.First().Id, Is.EqualTo(quests[0].Id));
                Assert.That(viewModel.LatestQuests.First().Title, Is.EqualTo(quests[0].Title));
                Assert.That(viewModel.LatestQuests.First().Description, Is.EqualTo(quests[0].Description));
                Assert.That(viewModel.LatestQuests.First().Created, Is.EqualTo(quests[0].Created));
                Assert.That(viewModel.LatestQuests.First().RewardAmount, Is.EqualTo(quests[0].RewardAmount));
                Assert.That(viewModel.LatestQuests.First().Type, Is.EqualTo(quests[0].Type.ToString()));
                Assert.That(viewModel.LatestQuests.First().ImageUrl, Is.EqualTo(quests[0].ImageName));
                Assert.That(viewModel.LatestQuests.Any(q => q.Description == "No description available"), Is.True);
            });


            Assert.That(viewModel.LatestMounts, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.LatestMounts.Count(), Is.EqualTo(mounts.Count));
                Assert.That(viewModel.LatestMounts.First().Id, Is.EqualTo(mounts[0].Id));
                Assert.That(viewModel.LatestMounts.First().Name, Is.EqualTo(mounts[0].Name));
                Assert.That(viewModel.LatestMounts.First().Description, Is.EqualTo(mounts[0].Description));
                Assert.That(viewModel.LatestMounts.First().RentPrice, Is.EqualTo(mounts[0].RentPrice));
                Assert.That(viewModel.LatestMounts.First().ImageUrl, Is.EqualTo(mounts[0].ImageUrl));
                Assert.That(viewModel.LatestMounts.First().Type, Is.EqualTo(mounts[0].MountType.ToString()));
                Assert.That(viewModel.LatestMounts.First().Rating, Is.EqualTo(mounts[0].Rating));
                Assert.That(viewModel.LatestMounts.Any(m => m.Description == "No description available"), Is.True);
            });


            Assert.That(viewModel.CheapestProducts, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.CheapestProducts.Count(), Is.EqualTo(products.Count));
                Assert.That(viewModel.CheapestProducts.First().Id, Is.EqualTo(products[0].Id));
                Assert.That(viewModel.CheapestProducts.First().Name, Is.EqualTo(products[0].Name));
                Assert.That(viewModel.CheapestProducts.First().Description, Is.EqualTo(products[0].Description));
                Assert.That(viewModel.CheapestProducts.First().Price, Is.EqualTo(products[0].Price));
                Assert.That(viewModel.CheapestProducts.First().ImageUrl, Is.EqualTo(products[0].ImageUrl));
                Assert.That(viewModel.CheapestProducts.First().ProductType, Is.EqualTo(products[0].ProductType.ToString()));
                Assert.That(viewModel.CheapestProducts.Any(p => p.Description == "No description available"), Is.True);
            });
        }

        [Test]
        public async Task CreateHomePageViewModelAsync_NoQuestsReturned_ViewModelHasEmptyQuestsList()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(new List<QuestEntity>());

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<MountEntity, DateTime>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(new List<MountEntity> { new() });

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<ProductEntity, decimal>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(new List<ProductEntity> { new() });

            // Act
            var viewModel = await _homeService.CreateHomePageViewModelAsync();

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.LatestQuests, Is.Empty);
                Assert.That(viewModel.LatestMounts, Is.Not.Empty);
                Assert.That(viewModel.CheapestProducts, Is.Not.Empty);
            });
        }

        [Test]
        public async Task CreateHomePageViewModelAsync_NoMountsReturned_ViewModelHasEmptyMountsList()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<MountEntity, DateTime>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(new List<MountEntity>());

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(new List<QuestEntity> { new() });

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<ProductEntity, decimal>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(new List<ProductEntity> { new() });

            // Act
            var viewModel = await _homeService.CreateHomePageViewModelAsync();

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.LatestMounts, Is.Empty);
                Assert.That(viewModel.LatestQuests, Is.Not.Empty);
                Assert.That(viewModel.CheapestProducts, Is.Not.Empty);
            });
        }

        [Test]
        public async Task CreateHomePageViewModelAsync_NoProductsReturned_ViewModelHasEmptyProductsList()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<ProductEntity, decimal>>>(),
                It.IsAny<Expression<Func<ProductEntity, bool>>>()
            )).ReturnsAsync(new List<ProductEntity>());

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(new List<QuestEntity> { new() });

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(),
                It.IsAny<Expression<Func<MountEntity, DateTime>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(new List<MountEntity> { new() });

            // Act
            var viewModel = await _homeService.CreateHomePageViewModelAsync();

            // Assert
            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.CheapestProducts, Is.Empty);
                Assert.That(viewModel.LatestQuests, Is.Not.Empty);
                Assert.That(viewModel.LatestMounts, Is.Not.Empty);
            });
        }
    }
}