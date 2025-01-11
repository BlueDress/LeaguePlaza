using System.Text.Json;

using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestApiController(IQuestService questService) : ControllerBase
    {
        private readonly IQuestService _questService = questService;

        [HttpPost("createquest")]
        public async Task<IActionResult> CreateQuest([FromBody] CreateQuestDto createQuestDto)
        {
            QuestDto newQuest = await _questService.CreateQuestAsync(createQuestDto);

            return Ok(JsonSerializer.Serialize(newQuest));
        }

        [HttpPut("updatequest")]
        public async Task<IActionResult> UpdateQuest([FromBody] UpdateQuestDataDto updateQuestDto)
        {
            QuestDto updatedQuest = await _questService.UpdateQuestAsync(updateQuestDto);

            return Ok(JsonSerializer.Serialize(updatedQuest));
        }

        [HttpPut("completequest")]
        public async Task<IActionResult> CompleteQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            await _questService.CompleteQuestAsync(updateQuestStatusDto.Id);

            return Ok();
        }

        [HttpPut("abandonQuest")]
        public async Task<IActionResult> AbandonQuest([FromBody] UpdateQuestStatusDto updateQuestStatusDto)
        {
            await _questService.AbandonQuestAsync(updateQuestStatusDto.Id);

            return Ok();
        }
    }
}
