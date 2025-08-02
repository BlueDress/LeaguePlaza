using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using LeaguePlaza.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.QuestConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Quest
{
    public class QuestApiController(IQuestService questService, ILogger<QuestApiController> logger) : BaseApiController
    {
        private readonly IQuestService _questService = questService;
        private readonly ILogger<QuestApiController> _logger = logger;

        [Authorize(Roles = QuestGiver)]
        [HttpPost("createquest")]
        public async Task<IActionResult> CreateQuest([FromForm] CreateQuestDto createQuestDto)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new QuestsViewModel());
                }

                await _questService.CreateQuestAsync(createQuestDto, currentUserId);
                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync(currentUserId);

                return PartialView(QuestCardsContainerWithPagination, questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(CreateQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = QuestGiver)]
        [HttpPut("updatequest")]
        public async Task<IActionResult> UpdateQuest([FromForm] UpdateQuestDataDto updateQuestDto)
        {
            try
            {
                await _questService.UpdateQuestAsync(updateQuestDto);

                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new QuestsViewModel());
                }

                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync(currentUserId);

                return PartialView(QuestCardsContainerWithPagination, questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(UpdateQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = Adventurer)]
        [HttpPut("acceptquest")]
        public async Task<IActionResult> AcceptQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return BadRequest();
                }

                await _questService.AcceptQuestAsync(updateQuestStatusDto.Id, currentUserId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(AcceptQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = QuestGiver)]
        [HttpDelete("removequest")]
        public async Task<IActionResult> RemoveQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            try
            {
                await _questService.RemoveQuestAsync(updateQuestStatusDto.Id);

                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new QuestsViewModel());
                }

                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync(currentUserId);

                return PartialView(QuestCardsContainerWithPagination, questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(RemoveQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = QuestGiver)]
        [HttpPut("completequest")]
        public async Task<IActionResult> CompleteQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            try
            {
                await _questService.CompleteQuestAsync(updateQuestStatusDto.Id);

                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new QuestsViewModel());
                }

                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync(currentUserId);

                return PartialView(QuestCardsContainerWithPagination, questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(CompleteQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = Adventurer)]
        [HttpPut("abandonquest")]
        public async Task<IActionResult> AbandonQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            try
            {
                await _questService.AbandonQuestAsync(updateQuestStatusDto.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(AbandonQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("filterandsortquests")]
        public async Task<IActionResult> FilterAndSortQuests([FromQuery] FilterAndSortQuestsRequestData filterAndSortQuestsRequestData)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                QuestsViewModel questsViewModel = await _questService.CreateQuestCardsContainerWithPaginationViewModelAsync(filterAndSortQuestsRequestData, currentUserId);

                return PartialView(QuestCardsContainerWithPagination, questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(FilterAndSortQuests));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
