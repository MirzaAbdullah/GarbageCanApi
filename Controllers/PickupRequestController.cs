using GarbageCanApi.Interfaces;
using GarbageCanApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickupRequestController : ControllerBase
    {
        private readonly IPickupRequest IRequestServices;
        private readonly ILogger<PickupRequestController> _logger;

        /// <summary>
        /// Constructor method resposible for DI between interfaces
        /// </summary>
        /// <param name="IRequestServices">Pass interface class of type ISuecrity which is responsbile for all security related operations</param>
        /// <param name="logger">Interface for NLog's</param>
        public PickupRequestController(IPickupRequest IRequestServices, ILogger<PickupRequestController> logger)
        {
            this.IRequestServices = IRequestServices;
            _logger = logger;
        }

        /// <summary>
        /// Update Pickup Status
        /// </summary>
        /// <param name="reqModel"> Set requestId and Pickup Status </param>
        /// <returns>a flag to confirm pickup status is changed or not - 'true' if status are successfully changed.</returns>
        /// <response code="200">Pickup Status changed successfully </response>
        /// <response code="400">Error while updating Pickup Status.</response>
        [HttpPut]
        [Authorize]
        [Route("UpdatePickupStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePickupStatus([FromBody] PickupRequestViewModel reqModel)
        {
            try
            {
                var isPickupStatusChanged = IRequestServices.ChangeRequestStatus(reqModel.IdRequest, reqModel.PickupStatus);
                if (isPickupStatusChanged)
                {
                    return StatusCode(200);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Pickup Status.");
            }

            return BadRequest("Updating Pickup Status Failed. Please try again!");
        }
    }
}
