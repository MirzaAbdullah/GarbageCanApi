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
    public class AssignController : ControllerBase
    {
        private readonly IAssign IAssignServices;
        private readonly ILogger<AssignController> _logger;

        /// <summary>
        /// Constructor method resposible for DI between interfaces
        /// </summary>
        /// <param name="IAssignServices">Pass interface class of type ISuecrity which is responsbile for all security related operations</param>
        /// <param name="logger">Interface for NLog's</param>
        public AssignController(IAssign IAssignServices, ILogger<AssignController> logger)
        {
            this.IAssignServices = IAssignServices;
            _logger = logger;
        }

        /// <summary>
        /// Get All Assign Pickups By DriverId 
        /// </summary>
        /// <param name="DriverId"> Set Driver Id </param>
        /// <returns>a model is send back if record exists</returns>
        /// <response code="200">If Record Exists </response>
        /// <response code="400">Error is there is no record.</response>
        [HttpGet]
        [Authorize]
        [Route("GetAllAssignPickupsByDriverId/{DriverId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllAssignPickupsByDriverId(string DriverId)
        {
            try
            {
                var isRecordExists = IAssignServices.GetAllAssignPickupsByDriverId(DriverId);
                if (isRecordExists != null)
                {
                    return Ok(isRecordExists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving Assign Pickups By DriverId");
            }

            return BadRequest("Retrieving Assign Pickups By DriverId failed. Please try again!");
        }

        /// <summary>
        /// Get Assign Pickup Data By Assign Id
        /// </summary>
        /// <param name="AssignId"> Set Assign Id </param>
        /// <returns>a model is send back if record exists</returns>
        /// <response code="200">If Record Exists </response>
        /// <response code="400">Error is there is no record.</response>
        [HttpGet]
        [Authorize]
        [Route("GetAssignPickupsById/{AssignId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAssignPickupsById(string AssignId)
        {
            try
            {
                var isRecordExists = IAssignServices.GetAssignPickupsById(AssignId);
                if (isRecordExists != null)
                {
                    return Ok(isRecordExists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving Assign Pickups By Assign Id");
            }

            return BadRequest("Retrieving Assign Pickups By Assign Id failed. Please try again!");
        }

        /// <summary>
        /// Get All In-Process Assign Pickup Data By Driver Id
        /// </summary>
        /// <param name="DriverId"> Set Assign Id </param>
        /// <returns>a model is send back if record exists</returns>
        /// <response code="200">If Record Exists </response>
        /// <response code="400">Error is there is no record.</response>
        [HttpGet]
        [Authorize]
        [Route("GetInProcAssignPickupsByDriverId/{DriverId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetInProcAssignPickupsByDriverId(string DriverId)
        {
            try
            {
                var isRecordExists = IAssignServices.GetInProcAssignPickupsByDriverId(DriverId);
                if (isRecordExists != null)
                {
                    return Ok(isRecordExists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving In-Process Assign Pickups By Driver Id");
            }

            return BadRequest("Retrieving In-Process Assign Pickups By Driver Id failed. Please try again!");
        }

        /// <summary>
        /// Accept Assign Request
        /// </summary>
        /// <param name="AssignModel"> Set AssignID </param>
        /// <returns>a flag is returned if Assignment is accepted as true else false.</returns>
        /// <response code="200">Accepted Assigned Request Successfuly </response>
        /// <response code="400">Error while accepting pickup request.</response>
        [HttpPut]
        [Authorize]
        [Route("AcceptAssignedRequestByDriver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AcceptAssignedRequestByDriver([FromBody] AssignViewModel AssignModel)
        {
            try
            {
                var isAssignRequestAccepted = IAssignServices.AcceptAssignedPickup(AssignModel.IdAssign);
                if (isAssignRequestAccepted)
                {
                    return Ok(isAssignRequestAccepted);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating an Assign request");
            }

            return BadRequest("Generating an assign request Failed. Please try again!");
        }

        /// <summary>
        /// Create Assign Request
        /// </summary>
        /// <param name="AssignModel"> Set ListRequestIds and userId </param>
        /// <returns>a model is send back if request is successfully created.</returns>
        /// <response code="200">Pickup Request created successfully </response>
        /// <response code="400">Error while creating a pickup request.</response>
        [HttpPost]
        [Authorize]
        [Route("CreatePickupRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreatePickupRequest([FromBody] AssignViewModel AssignModel)
        {
            try
            {
                var isAssignRequestGenerated = IAssignServices.AssignPickups(AssignModel.IdUser, AssignModel.ListRequestIds);
                if (isAssignRequestGenerated)
                {
                    return Ok(isAssignRequestGenerated);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating an Assign request");
            }

            return BadRequest("Generating an assign request Failed. Please try again!");
        }

    }
}
