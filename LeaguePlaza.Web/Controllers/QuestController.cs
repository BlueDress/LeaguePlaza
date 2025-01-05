using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers
{
    public class QuestController(IQuestService questService) : Controller
    {
        private readonly IQuestService _questService = questService;

        public async Task<IActionResult> Index()
        {
            AvailableQuestsViewModel availableQuestsViewModel = await _questService.CreateAvailableQuestsViewModelAsync();

            return View(availableQuestsViewModel);
        }

        public async Task<IActionResult> MyQuests()
        {
            UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

            return View(userQuestsViewModel);
        }
    }
}
