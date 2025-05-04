using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Quest
{
    public class QuestController(IQuestService questService, ILogger<QuestController> logger) : Controller
    {
        private readonly IQuestService _questService = questService;
        private readonly ILogger<QuestController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                AvailableQuestsViewModel availableQuestsViewModel = await _questService.CreateAvailableQuestsViewModelAsync();

                return View(availableQuestsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(Index));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new AvailableQuestsViewModel());
            }
        }

        [Authorize]
        public async Task<IActionResult> MyQuests()
        {
            try
            {
                UserQuestsViewModel userQuestsViewModel = await _questService.CreateUserQuestsViewModelAsync();

                return View(userQuestsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(MyQuests));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new UserQuestsViewModel());
            }
        }

        [Authorize]
        public async Task<IActionResult> ViewQuest(int id)
        {
            try
            {
                ViewQuestViewModel viewQuestViewModel = await _questService.CreateViewQuestViewModelAsync(id);

                return View(viewQuestViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(ViewQuest));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new ViewQuestViewModel());
            }
        }
    }
}
