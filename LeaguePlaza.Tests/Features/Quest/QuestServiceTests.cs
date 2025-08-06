using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Services;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using Moq;
using System.Linq.Expressions;

namespace LeaguePlaza.Tests.Features.Quest
{
    [TestFixture]
    public class QuestServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private Mock<IDropboxService> _mockDropboxService;
        private QuestService _questService;

        private const string GatheringDefaultImageUrl = "https://www.dropbox.com/scl/fi/ns7u5n9zhqw9q3i5g6gsq/gathering-default.jpg?rlkey=zbrno8iqnhxdqgmm2xkg8moyh&st=gm6ja4j6&raw=1";

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _mockDropboxService = new Mock<IDropboxService>();
            _questService = new QuestService(_mockRepository.Object, _mockDropboxService.Object);
        }

        [Test]
        public async Task CreateAvailableQuestsViewModelAsync_ReturnsQuestsWithPostedStatus()
        {
            // Arrange
            var quests = new List<QuestEntity>
            {
                new QuestEntity { Id = 1, Title = "Quest A", Status = QuestStatus.Posted, Created = DateTime.UtcNow.AddDays(-1) },
                new QuestEntity { Id = 2, Title = "Quest B", Status = QuestStatus.Posted, Created = DateTime.UtcNow.AddDays(-2) }
            };
            int totalResults = 10;

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, true,
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(quests);
            _mockRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<QuestEntity, bool>>>()))
                           .ReturnsAsync(totalResults);

            // Act
            var viewModel = await _questService.CreateAvailableQuestsViewModelAsync();

            // Assert
            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, true,
                It.IsAny<Expression<Func<QuestEntity, DateTime>>>(),
                It.Is<Expression<Func<QuestEntity, bool>>>(expr => expr.Compile()(new QuestEntity { Status = QuestStatus.Posted }))
            ), Times.Once);
            _mockRepository.Verify(r => r.GetCountAsync(It.Is<Expression<Func<QuestEntity, bool>>>(expr => expr.Compile()(new QuestEntity { Status = QuestStatus.Posted }))), Times.Once);

            Assert.That(viewModel, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Quests.Count(), Is.EqualTo(2));
                Assert.That(viewModel.Quests.First().Title, Is.EqualTo("Quest A"));
                Assert.That(viewModel.Quests.All(q => q.Status == QuestStatus.Posted.ToString()), Is.True);
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task CreateUserQuestsViewModelAsync_ReturnsQuestsForCurrentUser()
        {
            // Arrange
            string userId = "user123";
            var quests = new List<QuestEntity>
            {
                new QuestEntity { Id = 1, Title = "My Posted Quest", CreatorId = userId, Status = QuestStatus.Posted },
                new QuestEntity { Id = 2, Title = "My Accepted Quest", AdventurerId = userId, Status = QuestStatus.Accepted }
            };
            int totalResults = 2;

            _mockRepository.Setup(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, true,
                It.IsAny<Expression<Func<QuestEntity, object>>>(),
                It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(quests);
            _mockRepository.Setup(r => r.GetCountAsync(It.IsAny<Expression<Func<QuestEntity, bool>>>()))
                           .ReturnsAsync(totalResults);

            // Act
            var viewModel = await _questService.CreateUserQuestsViewModelAsync(userId);

            // Assert
            _mockRepository.Verify(r => r.FindSpecificCountOrderedReadOnlyAsync(
                1, 6, true,
                It.IsAny<Expression<Func<QuestEntity, object>>>(),
                It.Is<Expression<Func<QuestEntity, bool>>>(expr => expr.Compile()(new QuestEntity { CreatorId = userId }))
            ), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Quests.Count(), Is.EqualTo(2));
                Assert.That(viewModel.Quests.All(q => q.ShowExtraButtons), Is.True);
                Assert.That(viewModel.Pagination.TotalPages, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task CreateViewQuestViewModelAsync_QuestFound_ReturnsViewModelWithRecommendations()
        {
            // Arrange
            int questId = 1;
            string userId = "user123";
            var quest = new QuestEntity { Id = questId, Title = "Main Quest", Type = QuestType.MonsterHunt, Status = QuestStatus.Posted };
            var recommendedQuests = new List<QuestEntity>
            {
                new QuestEntity { Id = 2, Title = "Rec Quest 1", Type = QuestType.MonsterHunt, Status = QuestStatus.Posted },
                new QuestEntity { Id = 3, Title = "Rec Quest 2", Type = QuestType.MonsterHunt, Status = QuestStatus.Posted }
            };

            _mockRepository.Setup(r => r.FindByIdAsync<QuestEntity>(questId)).ReturnsAsync(quest);
            _mockRepository.Setup(r => r.FindSpecificCountReadOnlyAsync(
                3, It.IsAny<Expression<Func<QuestEntity, bool>>>()
            )).ReturnsAsync(recommendedQuests);

            // Act
            var viewModel = await _questService.CreateViewQuestViewModelAsync(questId, userId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(viewModel.Quest.Title, Is.EqualTo("Main Quest"));
                Assert.That(viewModel.RecommendedQuests.Count(), Is.EqualTo(2));
                Assert.That(viewModel.RecommendedQuests.All(q => q.Type == QuestType.MonsterHunt.ToString() && q.Status == QuestStatus.Posted.ToString()), Is.True);
                Assert.That(viewModel.CurrentUserId, Is.EqualTo(userId));
            });
        }

        [Test]
        public async Task CreateQuestAsync_NoImage_UsesDefaultUrl()
        {
            // Arrange
            var createDto = new CreateQuestDto
            {
                Title = "New Quest",
                Type = "1",
                Image = null
            };
            string userId = "user123";

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<QuestEntity>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _questService.CreateQuestAsync(createDto, userId);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<QuestEntity>(q =>
                q.Title == "New Quest" &&
                q.ImageName == GatheringDefaultImageUrl &&
                q.Status == QuestStatus.Posted
            )), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateQuestAsync_QuestFound_UpdatesProperties()
        {
            // Arrange
            int questId = 1;
            var questToUpdate = new QuestEntity { Id = questId, Title = "Old Title", ImageName = "old.jpg", Type = QuestType.MonsterHunt };
            var updateDto = new UpdateQuestDataDto
            {
                Id = questId,
                Title = "New Title",
                Type = "2",
                Image = null
            };

            _mockRepository.Setup(r => r.FindByIdAsync<QuestEntity>(questId)).ReturnsAsync(questToUpdate);
            _mockRepository.Setup(r => r.Update(questToUpdate)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _questService.UpdateQuestAsync(updateDto);

            // Assert
            _mockRepository.Verify(r => r.Update(questToUpdate), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(questToUpdate.Title, Is.EqualTo("New Title"));
                Assert.That(questToUpdate.Type, Is.EqualTo(QuestType.Escort));
            });
        }

        [Test]
        public async Task AcceptQuestAsync_QuestFound_UpdatesStatusAndAdventurerId()
        {
            // Arrange
            int questId = 1;
            string userId = "adventurer1";
            var questToAccept = new QuestEntity { Id = questId, Status = QuestStatus.Posted, AdventurerId = null };

            _mockRepository.Setup(r => r.FindByIdAsync<QuestEntity>(questId)).ReturnsAsync(questToAccept);
            _mockRepository.Setup(r => r.Update(questToAccept)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _questService.AcceptQuestAsync(questId, userId);

            // Assert
            _mockRepository.Verify(r => r.Update(questToAccept), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(questToAccept.Status, Is.EqualTo(QuestStatus.Accepted));
                Assert.That(questToAccept.AdventurerId, Is.EqualTo(userId));
            });
        }

        [Test]
        public async Task CompleteQuestAsync_QuestFound_UpdatesStatus()
        {
            // Arrange
            int questId = 1;
            var questToComplete = new QuestEntity { Id = questId, Status = QuestStatus.Accepted };

            _mockRepository.Setup(r => r.FindByIdAsync<QuestEntity>(questId)).ReturnsAsync(questToComplete);
            _mockRepository.Setup(r => r.Update(questToComplete)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _questService.CompleteQuestAsync(questId);

            // Assert
            _mockRepository.Verify(r => r.Update(questToComplete), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.That(questToComplete.Status, Is.EqualTo(QuestStatus.Completed));
        }

        [Test]
        public async Task AbandonQuestAsync_QuestFound_ResetsStatusAndAdventurerId()
        {
            // Arrange
            int questId = 1;
            var questToAbandon = new QuestEntity { Id = questId, Status = QuestStatus.Accepted, AdventurerId = "adventurer1" };

            _mockRepository.Setup(r => r.FindByIdAsync<QuestEntity>(questId)).ReturnsAsync(questToAbandon);
            _mockRepository.Setup(r => r.Update(questToAbandon)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _questService.AbandonQuestAsync(questId);

            // Assert
            _mockRepository.Verify(r => r.Update(questToAbandon), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Multiple(() =>
            {
                Assert.That(questToAbandon.Status, Is.EqualTo(QuestStatus.Posted));
                Assert.That(questToAbandon.AdventurerId, Is.Null);
            });
        }

        [Test]
        public async Task RemoveQuestAsync_QuestFound_RemovesAndSaves()
        {
            // Arrange
            int questId = 1;
            var questToRemove = new QuestEntity { Id = questId };

            _mockRepository.Setup(r => r.FindByIdAsync<QuestEntity>(questId)).ReturnsAsync(questToRemove);
            _mockRepository.Setup(r => r.Remove(questToRemove)).Verifiable();
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _questService.RemoveQuestAsync(questId);

            // Assert
            _mockRepository.Verify(r => r.FindByIdAsync<QuestEntity>(questId), Times.Once);
            _mockRepository.Verify(r => r.Remove(questToRemove), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
