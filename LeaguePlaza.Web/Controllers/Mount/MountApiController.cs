using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Mount
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleConstants.Adventurer)]
    public class MountApiController(IMountService mountService, ILogger<MountController> logger) : Controller
    {
        private const string MountCardsContainerWithPagination = "~/Views/Mount/Partials/_MountCardsContainerWithPagination.cshtml";
        private const string MountRentHistoryContainerWithPagination = "~/Views/Mount/Partials/_MountRentHistoryContainerWithPagination.cshtml";

        private readonly IMountService _mountService = mountService;
        private readonly ILogger<MountController> _logger = logger;

        [HttpGet("filterandsortmounts")]
        public async Task<IActionResult> FilterAndSortMounts([FromQuery] FilterAndSortMountsRequestData filterAndSortMountsRequestData)
        {
            try
            {
                MountCardsContainerWithPaginationViewModel mountCardsContainerWithPaginationViewModel = await _mountService.CreateMountCardsContainerWithPaginationViewModelAsync(filterAndSortMountsRequestData);

                return PartialView(MountCardsContainerWithPagination, mountCardsContainerWithPaginationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(FilterAndSortMounts));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpPost("rentmount")]
        public async Task<IActionResult> RentMount([FromBody] RentMountDto rentMountDto)
        {
            try
            {
                string response = await _mountService.RentMountAsync(rentMountDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(RentMount));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpPost("ratemount")]
        public async Task<IActionResult> RateMount([FromBody] RateMountDto rateMountDto)
        {
            try
            {
                string response = await _mountService.AddOrUpadeMountRatingAsync(rateMountDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(RateMount));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpDelete("cancelmountrent/{id}")]
        public async Task<IActionResult> CancelMountRent(int id)
        {
            try
            {
                await _mountService.CancelMountRentAsync(id);

                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(RentMount));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("getpageresults")]
        public async Task<IActionResult> GetPageResults([FromQuery] int pageNumber)
        {
            try
            {
                MountRentHistoryViewModel mountRentHistoryViewModel = await _mountService.CreateMountRentHistoryViewModelAsync(pageNumber);

                return PartialView(MountRentHistoryContainerWithPagination, mountRentHistoryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(GetPageResults));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new MountRentHistoryViewModel());
            }
        }
    }
}
