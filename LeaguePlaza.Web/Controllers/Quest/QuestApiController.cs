using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Quest
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestApiController(IQuestService questService, ILogger<QuestApiController> logger) : Controller
    {
        private const string QuestCardsContainerWithPagination = "~/Views/Quest/Partials/_QuestCardsContainerWithPagination.cshtml";

        private readonly IQuestService _questService = questService;
        private readonly ILogger<QuestApiController> _logger = logger;

        [Authorize(Roles = QuestGiver)]
        [HttpPost("createquest")]
        public async Task<IActionResult> CreateQuest([FromForm] CreateQuestDto createQuestDto)
        {
            try
            {
                await _questService.CreateQuestAsync(createQuestDto);
                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync();

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
                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync();

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
                await _questService.AcceptQuestAsync(updateQuestStatusDto.Id);

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
                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync();

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
                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync();

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
                QuestsViewModel questsViewModel = await _questService.CreateQuestCardsContainerWithPaginationViewModelAsync(filterAndSortQuestsRequestData);

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
