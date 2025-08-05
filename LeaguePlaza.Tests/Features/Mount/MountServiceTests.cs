using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Services;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;

namespace LeaguePlaza.Tests.Features.Mount
{
    [TestFixture]
    public class MountServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDropboxService> _mockDropboxService;
        private MountService _mountService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _mockDropboxService = new Mock<IDropboxService>();
            _mountService = new MountService(_mockRepository.Object, _mockDropboxService.Object);
        }

        [Test]
        public async Task CreateMountsViewModelAsync_ReturnsCorrectViewModelWithTopRatedMounts()
        {
            // Arrange
            var mounts = new List<MountEntity>
            {
                new MountEntity { Id = 1, Name = "Mount A", Description = "Desc A", RentPrice = 10m, ImageUrl = "urlA", MountType = MountType.Ground, Rating = 4.5 },
                new MountEntity { Id = 2, Name = "Mount B", Description = "", RentPrice = 20m, ImageUrl = "urlB", MountType = MountType.Flying, Rating = 3.2 }
            };
            int totalResults = 10;

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, true,
                It.IsAny<Expression<Func<MountEntity, double>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(mounts);
            _mockRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<MountEntity, bool>>>()))
                           .ReturnsAsync(totalResults);

            // Act
            var viewModel = await _mountService.CreateMountsViewModelAsync();

            // Assert
            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, true,
                It.IsAny<Expression<Func<MountEntity, double>>>(),
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            ), Times.Once);
            _mockRepository.Verify(r => r.GetCountAsync(It.IsAny<Expression<Func<MountEntity, bool>>>()), Times.Once);

            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Mounts.Count(), Is.EqualTo(2));
                Assert.That(viewModel.Mounts.First().Name, Is.EqualTo("Mount A"));
                Assert.That(viewModel.Mounts.Last().Description, Is.EqualTo("No description available"));
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(1));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task CreateViewMountViewModelAsync_MountFound_ReturnsCorrectViewModelWithUserRatingAndRecommendations()
        {
            // Arrange
            int mountId = 1;
            string userId = "user123";
            var mount = new MountEntity
            {
                Id = mountId,
                Name = "Mount A",
                Description = "A detailed description.",
                RentPrice = 50m,
                ImageUrl = "mount.jpg",
                MountType = MountType.Flying,
                Rating = 4.0,
                MountRatings = new List<MountRatingEntity>
                {
                    new MountRatingEntity { Id = 1, Rating = 5, UserId = userId, MountId = mountId },
                    new MountRatingEntity { Id = 2, Rating = 3, UserId = "otherUser", MountId = mountId }
                }
            };
            var recommendedMounts = new List<MountEntity>
            {
                new MountEntity { Id = 2, MountType = MountType.Flying },
                new MountEntity { Id = 3, MountType = MountType.Flying }
            };

            _mockRepository.Setup(r => r.FindOneReadOnlyAsync(
                It.IsAny<Expression<Func<MountEntity, bool>>>(),
                It.IsAny<Func<IQueryable<MountEntity>, IIncludableQueryable<MountEntity, object>>>()
            )).ReturnsAsync(mount);

            _mockRepository.Setup(r => r.FindSpecificCountReadOnlyAsync(
                3, It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(recommendedMounts);

            // Act
            var viewModel = await _mountService.CreateViewMountViewModelAsync(mountId, userId);

            // Assert
            _mockRepository.Verify(r => r.FindOneReadOnlyAsync(
                It.Is<Expression<Func<MountEntity, bool>>>(expr => expr.Compile()(mount)),
                It.IsAny<Func<IQueryable<MountEntity>, IIncludableQueryable<MountEntity, object>>>()
            ), Times.Once);
            _mockRepository.Verify(r => r.FindSpecificCountReadOnlyAsync(
                3,
                It.Is<Expression<Func<MountEntity, bool>>>(expr => expr.Compile()(new MountEntity { Id = 0, MountType = MountType.Flying }))
            ), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Mount.Name, Is.EqualTo("Mount A"));
                Assert.That(viewModel.UserRating, Is.EqualTo(5));
                Assert.That(viewModel.RecommendedMounts.Count(), Is.EqualTo(2));
                Assert.That(viewModel.RecommendedMounts.All(m => m.Type == MountType.Flying.ToString()), Is.True);
            });
        }

        [Test]
        public async Task CreateMountRentHistoryViewModelAsync_ReturnsCorrectViewModelWithRentalsAndPagination()
        {
            // Arrange
            string userId = "user123";
            int pageNumber = 2;
            int totalResults = 15;
            var mounts = new List<MountRentalEntity>
            {
                new MountRentalEntity { Id = 1, UserId = userId, MountId = 1, StartDate = DateTime.Today.AddDays(-5), EndDate = DateTime.Today.AddDays(-3), Mount = new MountEntity { Name = "Mount A" } },
                new MountRentalEntity { Id = 2, UserId = userId, MountId = 2, StartDate = DateTime.Today.AddDays(-2), EndDate = DateTime.Today.AddDays(-1), Mount = new MountEntity { Name = "Mount B" } }
            };

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber, 10, false,
                It.IsAny<Expression<Func<MountRentalEntity, DateTime>>>(),
                It.IsAny<Expression<Func<MountRentalEntity, bool>>>(),
                It.IsAny<Func<IQueryable<MountRentalEntity>, IIncludableQueryable<MountRentalEntity, object>>>()
            )).ReturnsAsync(mounts);

            _mockRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<MountRentalEntity, bool>>>()
            )).ReturnsAsync(totalResults);

            // Act
            var viewModel = await _mountService.CreateMountRentHistoryViewModelAsync(userId, pageNumber);

            // Assert
            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                pageNumber, 10, false,
                It.IsAny<Expression<Func<MountRentalEntity, DateTime>>>(),
                It.Is<Expression<Func<MountRentalEntity, bool>>>(expr => expr.Compile()(new MountRentalEntity { UserId = userId })),
                It.IsAny<Func<IQueryable<MountRentalEntity>, IIncludableQueryable<MountRentalEntity, object>>>()
            ), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.MountRentals.Count(), Is.EqualTo(2));
                Assert.That(viewModel.MountRentals.First().MountName, Is.EqualTo("Mount A"));
                Assert.That(viewModel.Pagination.CurrentPage, Is.EqualTo(pageNumber));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task CreateMountsViewModelAsync_InvalidDateRange_ReturnsEmptyViewModel()
        {
            // Arrange
            var requestData = new FilterAndSortMountsRequestData
            {
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            // Act
            var viewModel = await _mountService.CreateMountsViewModelAsync(requestData);

            // Assert
            _mockRepository.Verify(r => r.GetCountAsync(It.IsAny<Expression<Func<MountEntity, bool>>>()), Times.Never);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Mounts, Is.Empty);
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task CreateMountsViewModelAsync_WithValidFiltersAndDates_ReturnsFilteredViewModel()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var requestData = new FilterAndSortMountsRequestData
            {
                SearchTerm = "A",
                TypeFilters = "0,1",
                StartDate = today.AddDays(10),
                EndDate = today.AddDays(15),
                SortBy = "Price",
                OrderIsDescending = false,
                CurrentPage = 1
            };

            var mounts = new List<MountEntity>
            {
                new MountEntity { Id = 1, Name = "Mount A", RentPrice = 10m, MountType = MountType.Ground, MountRentals = new List<MountRentalEntity>() },
                new MountEntity { Id = 2, Name = "Mount B", RentPrice = 20m, MountType = MountType.Flying, MountRentals = new List<MountRentalEntity>() },
            };
            int totalResults = 2;

            _mockRepository.Setup(r => r.GetCountAsync(
                It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(totalResults);
            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<Expression<Func<MountEntity, object>>>(), It.IsAny<Expression<Func<MountEntity, bool>>>()
            )).ReturnsAsync(mounts);

            // Act
            var viewModel = await _mountService.CreateMountsViewModelAsync(requestData);

            // Assert
            _mockRepository.Verify(r => r.GetCountAsync(
                It.Is<Expression<Func<MountEntity, bool>>>(expr =>
                    expr.Compile()(new MountEntity { Name = "Mount A", MountType = MountType.Ground, MountRentals = new List<MountRentalEntity>() })
                )
            ), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Mounts.Count(), Is.EqualTo(2));
                Assert.That(viewModel.Mounts.First().Name, Is.EqualTo("Mount A"));
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task RentMountAsync_WithValidDatesAndAvailability_Succeeds()
        {
            // Arrange
            var rentDto = new RentMountDto
            {
                MountId = 1,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(15)
            };
            var mount = new MountEntity { Id = 1 };
            var existingRentals = new List<MountRentalEntity>
            {
                new MountRentalEntity { StartDate = DateTime.UtcNow.AddDays(5), EndDate = DateTime.UtcNow.AddDays(7) },
            };

            _mockRepository.Setup(r => r.FindByIdAsync<MountEntity>(rentDto.MountId)).ReturnsAsync(mount);
            _mockRepository.Setup(r => r.FindAllReadOnlyAsync(
                It.IsAny<Expression<Func<MountRentalEntity, bool>>>()
            )).ReturnsAsync(existingRentals);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MountRentalEntity>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _mountService.RentMountAsync(rentDto, "user123");

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MountRentalEntity>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsMountRentSuccessful, Is.True);
                Assert.That(result.MountRentMessage, Is.EqualTo("Mount rented successfully for the chosen interval"));
            });
        }

        [Test]
        public async Task RentMountAsync_MountNotAvailableForInterval_Fails()
        {
            // Arrange
            var rentDto = new RentMountDto
            {
                MountId = 1,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(15)
            };
            var mount = new MountEntity { Id = 1 };
            var existingRentals = new List<MountRentalEntity>
            {
                new MountRentalEntity { StartDate = DateTime.UtcNow.AddDays(12), EndDate = DateTime.UtcNow.AddDays(14) },
            };

            _mockRepository.Setup(r => r.FindByIdAsync<MountEntity>(rentDto.MountId)).ReturnsAsync(mount);
            _mockRepository.Setup(r => r.FindAllReadOnlyAsync(It.IsAny<Expression<Func<MountRentalEntity, bool>>>()))
                           .ReturnsAsync(existingRentals);

            // Act
            var result = await _mountService.RentMountAsync(rentDto, "user123");

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MountRentalEntity>()), Times.Never);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsMountRentSuccessful, Is.False);
                Assert.That(result.MountRentMessage, Is.EqualTo("The mount is not available for the chosen interval"));
            });
        }

        [Test]
        public async Task AddOrUpadeMountRatingAsync_UserRatesForFirstTime_AddsNewRatingAndRecalculatesAverage()
        {
            // Arrange
            var rateDto = new RateMountDto { MountId = 1, Rating = 4 };
            string userId = "user123";
            var mountToRate = new MountEntity { Id = 1, Rating = 5.0 };
            var currentRatings = new List<MountRatingEntity>
            {
                new MountRatingEntity { Rating = 5, UserId = "otherUser" }
            };

            _mockRepository.Setup(r => r.FindByIdAsync<MountEntity>(rateDto.MountId)).ReturnsAsync(mountToRate);
            _mockRepository.Setup(r => r.FindAllAsync(It.IsAny<Expression<Func<MountRatingEntity, bool>>>()))
                           .ReturnsAsync(currentRatings);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MountRatingEntity>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _mountService.AddOrUpadeMountRatingAsync(rateDto, userId);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<MountRatingEntity>(r => r.Rating == 4 && r.UserId == userId)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.That(mountToRate.Rating, Is.EqualTo(4.5));
        }

        [Test]
        public async Task AddOrUpadeMountRatingAsync_UserUpdatesExistingRating_UpdatesRatingAndRecalculatesAverage()
        {
            // Arrange
            var rateDto = new RateMountDto { MountId = 1, Rating = 2 };
            string userId = "user123";
            var mountToRate = new MountEntity { Id = 1, Rating = 5.0 };
            var existingUserRating = new MountRatingEntity { Id = 10, Rating = 5, UserId = userId };
            var currentRatings = new List<MountRatingEntity> { existingUserRating, new MountRatingEntity { Rating = 3, UserId = "otherUser" } };

            _mockRepository.Setup(r => r.FindByIdAsync<MountEntity>(rateDto.MountId)).ReturnsAsync(mountToRate);
            _mockRepository.Setup(r => r.FindAllAsync(It.IsAny<Expression<Func<MountRatingEntity, bool>>>()))
                           .ReturnsAsync(currentRatings);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MountRatingEntity>())).Returns(Task.CompletedTask);

            // Act
            await _mountService.AddOrUpadeMountRatingAsync(rateDto, userId);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MountRatingEntity>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.That(existingUserRating.Rating, Is.EqualTo(2));
            Assert.That(mountToRate.Rating, Is.EqualTo(2.5));
        }

        [Test]
        public async Task CancelMountRentAsync_RentalFound_RemovesAndSaves()
        {
            // Arrange
            int rentalId = 1;
            var rental = new MountRentalEntity { Id = rentalId };

            _mockRepository.Setup(r => r.FindByIdAsync<MountRentalEntity>(rentalId)).ReturnsAsync(rental);
            _mockRepository.Setup(r => r.Remove(rental)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _mountService.CancelMountRentAsync(rentalId);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<MountRentalEntity>(rentalId), Times.Once);
            _mockRepository.Verify(r => r.Remove(rental), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateMountsAsync_NoImage_CreatesMountWithDefaultImageUrl()
        {
            // Arrange
            var createDto = new CreateMountDto { Name = "No Image Mount", MountType = "1" };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MountEntity>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _mountService.CreateMountsAsync(createDto);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<MountEntity>(m =>
                m.Name == createDto.Name &&
                m.ImageUrl == "https://www.dropbox.com/scl/fi/9n7d7geaprae40gjhcvyr/flying-default.jpg?rlkey=ktap2t7jjgo2j8oatka34rxdu&st=pfuagymz&raw=1"
            )), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateMountAsync_NoNewImage_UpdatesMountButKeepsOldUrl()
        {
            // Arrange
            int mountId = 1;
            var mountToUpdate = new MountEntity { Id = mountId, Name = "Old Name", ImageUrl = "old.jpg", MountType = MountType.Ground };
            var updateDto = new UpdateMountDto { Id = mountId, Name = "New Name", MountType = "2", Image = null };

            _mockRepository.Setup(r => r.FindByIdAsync<MountEntity>(mountId)).ReturnsAsync(mountToUpdate);
            _mockRepository.Setup(r => r.Update(mountToUpdate)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _mountService.UpdateMountAsync(updateDto);

            // Assert
            _mockRepository.Verify(r => r.Update(mountToUpdate), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(mountToUpdate.Name, Is.EqualTo("New Name"));
                Assert.That(mountToUpdate.ImageUrl, Is.EqualTo("https://www.dropbox.com/scl/fi/soux4avtf2hlpjw2gguth/aquatic-default.jpg?rlkey=hlf1n9g4pts8zrcfiaiglddyu&st=om0epfjz&raw=1"));
                Assert.That(mountToUpdate.MountType, Is.EqualTo(MountType.Aquatic));
            });

        }

        [Test]
        public async Task DeleteMountAsync_MountFound_RemovesAndSaves()
        {
            // Arrange
            int mountId = 1;
            var mountToRemove = new MountEntity { Id = mountId };

            _mockRepository.Setup(r => r.FindByIdAsync<MountEntity>(mountId)).ReturnsAsync(mountToRemove);
            _mockRepository.Setup(r => r.Remove(mountToRemove)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _mountService.DeleteMountAsync(mountId);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<MountEntity>(mountId), Times.Once);
            _mockRepository.Verify(r => r.Remove(mountToRemove), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}

