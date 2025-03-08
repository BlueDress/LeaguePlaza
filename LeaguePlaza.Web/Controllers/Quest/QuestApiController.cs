using System.Text.Json;

using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Quest
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestApiController(IQuestService questService) : Controller
    {
        private readonly IQuestService _questService = questService;

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpPost("createquest")]
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestDto createQuestDto)
        {
            await _questService.CreateQuestAsync(createQuestDto);
            UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

            return PartialView("~/Views/Shared/Quest/_QuestCardsContainerWithPagination.cshtml", userQuestsViewModel.ViewModel);
        }

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpPut("updatequest")]
        public async Task<IActionResult> UpdateQuest([FromBody] UpdateQuestDataDto updateQuestDto)
        {
            QuestDto updatedQuest = await _questService.UpdateQuestAsync(updateQuestDto);

            return Ok(JsonSerializer.Serialize(updatedQuest));
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        [HttpPut("acceptquest")]
        public async Task<IActionResult> AcceptQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            await _questService.AcceptQuest(updateQuestStatusDto.Id);

            return Ok();
        }

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpDelete("removequest")]
        public async Task<IActionResult> RemoveQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            await _questService.RemoveQuestAsync(updateQuestStatusDto.Id);

            return Ok();
        }

        [Authorize(Roles = UserRoleConstants.QuestGiver)]
        [HttpPut("completequest")]
        public async Task<IActionResult> CompleteQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            await _questService.CompleteQuestAsync(updateQuestStatusDto.Id);

            return Ok();
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        [HttpPut("abandonQuest")]
        public async Task<IActionResult> AbandonQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            await _questService.AbandonQuestAsync(updateQuestStatusDto.Id);

            return Ok();
        }
    }
}
