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
            AllAvailableQuestsViewModel allAvailableQuestsViewModel = await _questService.CreateAllAvailableQuestsViewModel();

            return View(allAvailableQuestsViewModel);
        }
    }
}
