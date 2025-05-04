using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Quest
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestApiController(IQuestService questService, ILogger<QuestApiController> logger) : Controller
    {
        private const string QuestCardsContainerWithPagination = "~/Views/Shared/Quest/_QuestCardsContainerWithPagination.cshtml";

        private readonly IQuestService _questService = questService;
        private readonly ILogger<QuestApiController> _logger = logger;

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpPost("createquest")]
        public async Task<IActionResult> CreateQuest([FromForm] CreateQuestDto createQuestDto)
        {
            try
            {
                await _questService.CreateQuestAsync(createQuestDto);
                UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

                return PartialView(QuestCardsContainerWithPagination, userQuestsViewModel.ViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(CreateQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpPut("updatequest")]
        public async Task<IActionResult> UpdateQuest([FromForm] UpdateQuestDataDto updateQuestDto)
        {
            try
            {
                await _questService.UpdateQuestAsync(updateQuestDto);
                UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

                return PartialView(QuestCardsContainerWithPagination, userQuestsViewModel.ViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(UpdateQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
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
                _logger.LogError(ErrorConstants.FailedAt, nameof(AcceptQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpDelete("removequest")]
        public async Task<IActionResult> RemoveQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            try
            {
                await _questService.RemoveQuestAsync(updateQuestStatusDto.Id);
                UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

                return PartialView(QuestCardsContainerWithPagination, userQuestsViewModel.ViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(RemoveQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpPut("completequest")]
        public async Task<IActionResult> CompleteQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            try
            {
                await _questService.CompleteQuestAsync(updateQuestStatusDto.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(CompleteQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
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
                _logger.LogError(ErrorConstants.FailedAt, nameof(AbandonQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("filterandsortquests")]
        public async Task<IActionResult> FilterAndSortQuests([FromQuery] FilterAndSortQuestsRequestData filterAndSortQuestsRequestData)
        {
            try
            {
                QuestCardsContainerWithPaginationViewModel questCardsContainerWithPaginationViewModel = await _questService.CreateQuestCardsContainerWithPaginationViewModelAsync(filterAndSortQuestsRequestData);

                return PartialView(QuestCardsContainerWithPagination, questCardsContainerWithPaginationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(FilterAndSortQuests));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
