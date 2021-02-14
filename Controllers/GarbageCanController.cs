using GarbageCanApi.Interfaces;
using GarbageCanApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace GarbageCanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GarbageCanController : ControllerBase
    {
        private readonly IGarbageCan IGarbageCanServices;
        private readonly ILogger<GarbageCanController> _logger;

        /// <summary>
        /// Constructor method resposible for DI between interfaces
        /// </summary>
        /// <param name="IGarbageCanServices">Pass interface class of type ISuecrity which is responsbile for all security related operations</param>
        /// <param name="logger">Interface for NLog's</param>
        public GarbageCanController(IGarbageCan IGarbageCanServices, ILogger<GarbageCanController> logger)
        {
            this.IGarbageCanServices = IGarbageCanServices;
            _logger = logger;
        }

        /// <summary>
        /// Getting User Details By User Id
        /// </summary>
        /// <param name="userId"> pass id of the user for details </param>
        /// <returns>List of User-Detail depending on 'Id' Value</returns>
        /// <response code="302">Users Data</response>
        /// <response code="404">No Data Found</response>
        [HttpGet]
        [Authorize]
        [Route("GetUserDetailsById/{userId}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserDetailsById(string userId)
        {
            var userDetails = IGarbageCanServices.GetUserDetailsById(userId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }

            return NotFound();
        }

        /// <summary>
        /// List of all Items
        /// </summary>
        /// <returns>List of all Items</returns>
        /// <response code="302">Data Found</response>
        /// <response code="404">No Data Found</response>
        [HttpGet]
        [Authorize]
        [Route("GetAllItems")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllItems()
        {
            var itemsModel = IGarbageCanServices.ddlItems();
            if (itemsModel != null)
            {
                return Ok(itemsModel);
            }

            return NotFound();
        }

        /// <summary>
        /// List of all Roles
        /// </summary>
        /// <returns>List of all Roles</returns>
        /// <response code="302">Data Found</response>
        /// <response code="404">No Data Found</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetAllRoles")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllRoles()
        {
            var rolesModel = IGarbageCanServices.ddlRoles();
            if (rolesModel != null)
            {
                return Ok(rolesModel);
            }

            return NotFound();
        }

        /// <summary>
        /// Update User Details
        /// </summary>
        /// <param name="udModel"> Set Model to get updated </param>
        /// <returns>a flag to confirm user details is changed or not - 'true' if user details are successfully changed.</returns>
        /// <response code="200">User Details changed successfully </response>
        /// <response code="400">Error while changing user detail.</response>
        [HttpPut]
        [Authorize]
        [Route("UpdateUserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUserDetails([FromBody] UserDetailsViewModel udModel)
        {
            try
            {
                var isUserDetailsUpdated = IGarbageCanServices.UpdateUserDetails(udModel);
                if (isUserDetailsUpdated)
                {
                    return StatusCode(200);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user details.");
            }

            return BadRequest("Changing user details failed. Please try again");
        }

        /// <summary>
        /// Delete User Details
        /// </summary>
        /// <param name="udModel"> Set user id only </param>
        /// <returns>a flag to confirm user details is delete or not - 'true' if user details are successfully changed.</returns>
        /// <response code="200">User Details changed successfully </response>
        /// <response code="400">Error while changing user detail.</response>
        [HttpPut]
        [Authorize]
        [Route("DeleteUserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteUserDetails([FromBody] UserDetailsViewModel udModel)
        {
            try
            {
                var isUserDetailsDeleted = IGarbageCanServices.DeleteUserDetails(udModel);
                if (isUserDetailsDeleted)
                {
                    return StatusCode(200);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user details.");
            }

            return BadRequest("Deleting user details failed. Please try again");
        }

        /// <summary>
        /// Save User Details
        /// </summary>
        /// <param name="udModel"> Set the Complete Model </param>
        /// <returns>user model filled with details of newly added user</returns>
        /// <response code="200">User added successfully </response>
        /// <response code="400">Error while Adding User</response>
        /// <response code="500">Internal Server Error or Error while handshaking with database</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("CreateUserDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateUserDetails([FromBody] UserDetailsViewModel udModel)
        {
            try
            {
                var userDetails = IGarbageCanServices.CreateUserDetails(udModel);

                if (userDetails != null)
                {
                    return Ok(userDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user details.");
            }

            return BadRequest("Adding user details failed. Please try again.");
        }
    }
}
