using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

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
                QuestsViewModel questsViewModel = await _questService.CreateAvailableQuestsViewModelAsync();

                return View(questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(Index));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new QuestsViewModel());
            }
        }

        [Authorize(Roles = $"{Adventurer}, {QuestGiver}")]
        public async Task<IActionResult> MyQuests()
        {
            try
            {
                QuestsViewModel questsViewModel = await _questService.CreateUserQuestsViewModelAsync();

                return View(questsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(MyQuests));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new QuestsViewModel());
            }
        }

        [Authorize(Roles = $"{Adventurer}, {QuestGiver}")]
        public async Task<IActionResult> ViewQuest(int id)
        {
            try
            {
                ViewQuestViewModel viewQuestViewModel = await _questService.CreateViewQuestViewModelAsync(id);

                return View(viewQuestViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ViewQuest));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new ViewQuestViewModel());
            }
        }
    }
}
