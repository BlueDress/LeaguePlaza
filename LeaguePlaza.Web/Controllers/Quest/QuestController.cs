using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Quest
{
    public class QuestController(IQuestService questService) : Controller
    {
        private readonly IQuestService _questService = questService;

        public async Task<IActionResult> Index()
        {
            AvailableQuestsViewModel availableQuestsViewModel = await _questService.CreateAvailableQuestsViewModelAsync();

            return View(availableQuestsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> MyQuests()
        {
            UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

            return View(userQuestsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> ViewQuest(int id)
        {
            ViewQuestViewModel viewQuestViewModel = await _questService.CreateViewQuestViewModelAsync(id);

            return View(viewQuestViewModel);
        }
    }
}
