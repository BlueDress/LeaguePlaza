﻿using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.MountConstants;
using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Mount
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Adventurer)]
    public class MountApiController(IMountService mountService, ILogger<MountController> logger) : Controller
    {
        private readonly IMountService _mountService = mountService;
        private readonly ILogger<MountController> _logger = logger;

        [HttpGet("filterandsortmounts")]
        public async Task<IActionResult> FilterAndSortMounts([FromQuery] FilterAndSortMountsRequestData filterAndSortMountsRequestData)
        {
            try
            {
                MountsViewModel mountsViewModel = await _mountService.CreateMountsViewModelAsync(filterAndSortMountsRequestData);

                return PartialView(MountCardsContainerWithPagination, mountsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(FilterAndSortMounts));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpPost("rentmount")]
        public async Task<IActionResult> RentMount([FromBody] RentMountDto rentMountDto)
        {
            try
            {
                MountRentalResultDto mountRentResult = await _mountService.RentMountAsync(rentMountDto);

                return Ok(mountRentResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(RentMount));
                _logger.LogError(ErrorMessage, ex.Message);

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
                _logger.LogError(FailedAt, nameof(RateMount));
                _logger.LogError(ErrorMessage, ex.Message);

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
                _logger.LogError(FailedAt, nameof(RentMount));
                _logger.LogError(ErrorMessage, ex.Message);

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
                _logger.LogError(FailedAt, nameof(GetPageResults));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new MountRentHistoryViewModel());
            }
        }
    }
}
