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
    }
}
