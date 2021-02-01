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
        /// Get Pickup Request by Id
        /// </summary>
        /// <param name="requestId"> Set request Id </param>
        /// <returns>a model is send back if record exists</returns>
        /// <response code="200">If Record Exists </response>
        /// <response code="400">Error is there is no record.</response>
        [HttpGet]
        [Authorize]
        [Route("GetPickupRequestById/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPickupRequestById(string requestId)
        {
            try
            {
                var isPickupRequestExists = IRequestServices.GetRequestsById(requestId);
                if (isPickupRequestExists != null)
                {
                    return Ok(isPickupRequestExists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving pick up request");
            }

            return BadRequest("Retrieving pickup request failed. Please try again!");
        }

        /// <summary>
        /// Get Pickup Request by Pickup Status
        /// </summary>
        /// <param name="status"> Set Pickup Status </param>
        /// <returns>a model is send back if there are records of entered status</returns>
        /// <response code="200">If Record Exists </response>
        /// <response code="400">Error is there is no record.</response>
        [HttpGet]
        [Authorize]
        [Route("GetPickupRequestByStatus/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPickupRequestByStatus(string status)
        {
            try
            {
                var isPickupRequestExists = IRequestServices.GetAllRequestsByStatus(status);
                if (isPickupRequestExists != null)
                {
                    return Ok(isPickupRequestExists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving pick up request");
            }

            return BadRequest("Retrieving pickup request failed. Please try again!");
        }

        /// <summary>
        /// Get Pickup Request by User Id
        /// </summary>
        /// <param name="UserId"> Set User Id </param>
        /// <returns>a model is send back if there are records of entered status</returns>
        /// <response code="200">If Record Exists </response>
        /// <response code="400">Error is there is no record.</response>
        [HttpGet]
        [Authorize]
        [Route("GetPickupRequestByUserId/{UserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetPickupRequestByUserId(string UserId)
        {
            try
            {
                var isPickupRequestExists = IRequestServices.GetAllRequestsByUserId(UserId);
                if (isPickupRequestExists != null)
                {
                    return Ok(isPickupRequestExists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving pick up request");
            }

            return BadRequest("Retrieving pickup request failed. Please try again!");
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

        /// <summary>
        /// Update Request Details
        /// </summary>
        /// <param name="reqModel"> Set requestId, Pickup Cost and Request Details Table </param>
        /// <returns>a flag to confirm request details is changed or not - 'true' if details are successfully changed.</returns>
        /// <response code="200">Request Details changed successfully </response>
        /// <response code="400">Error while updating request details.</response>
        [HttpPut]
        [Authorize]
        [Route("UpdateRequestDetailsByDriver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateRequestDetailsByDriver([FromBody] PickupRequestViewModel reqModel)
        {
            try
            {
                var isPickupStatusChanged = IRequestServices.UpdateRequestDetailsByDriver(reqModel);
                if (isPickupStatusChanged)
                {
                    return StatusCode(200);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating Pickup Status.");
            }

            return BadRequest("Updating Request Details Failed. Please try again!");
        }

        /// <summary>
        /// Create Pickup Request
        /// </summary>
        /// <param name="reqModel"> Set requestId and Pickup Status </param>
        /// <returns>a model is send back if request is successfully created.</returns>
        /// <response code="200">Pickup Request created successfully </response>
        /// <response code="400">Error while creating a pickup request.</response>
        [HttpPost]
        [Authorize]
        [Route("CreatePickupRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreatePickupRequest([FromBody] PickupRequestViewModel reqModel)
        {
            try
            {
                var isPickupRequestGenerated = IRequestServices.CreatePickupRequest(reqModel);
                if (isPickupRequestGenerated != null)
                {
                    return Ok(isPickupRequestGenerated);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating a pickup request");
            }

            return BadRequest("Generating a pickup request Failed. Please try again!");
        }

        /// <summary>
        /// Delete Pickup Request by Pickup Request Id
        /// </summary>
        /// <param name="requestId"> Set Pickup Request Id </param>
        /// <returns>a Boolean flag is returned if Request Details are deleted</returns>
        /// <response code="200">If record deleted </response>
        /// <response code="400">Error, if record is not deleted.</response>
        [HttpDelete]
        [Authorize]
        [Route("DeletePickupRequest/{requestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeletePickupRequest(string requestId)
        {
            try
            {
                var isPickupRequestExists = IRequestServices.DeletePickupRequest(requestId);
                if (isPickupRequestExists)
                {
                    return Ok(true);
                }

                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting pick up request");
            }

            return BadRequest("Deleting pickup request failed. Please try again!");
        }
    }
}
