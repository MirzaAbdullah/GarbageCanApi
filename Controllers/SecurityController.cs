using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
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
    /// <summary>
    /// Controller responsible for all security methods such as creating users, generating tokens and much more
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurity ISecurityServices;
        private readonly ILogger<SecurityController> _logger;

        /// <summary>
        /// Constructor method resposible for DI between interfaces
        /// </summary>
        /// <param name="ISecurityServices">Pass interface class of type ISuecrity which is responsbile for all security related operations</param>
        /// <param name="logger">Interface for NLog's</param>
        public SecurityController(ISecurity ISecurityServices, ILogger<SecurityController> logger)
        {
            this.ISecurityServices = ISecurityServices;
            _logger = logger;
        }

        /// <summary>
        /// Getting all active users including User Name
        /// </summary>
        /// <returns>List of all Verified Users</returns>
        /// <response code="302">Users Data</response>
        /// <response code="404">No Data Found</response>
        [HttpGet]
        [Authorize]
        [Route("GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllUsers()
        {
            var allUsers = ISecurityServices.GetAllUsers().ToList();

            if (allUsers.Count > 0)
            {
                return Ok(allUsers);
            }

            return NotFound();
        }

        /// <summary>
        /// Getting active user including User Name
        /// </summary>
        /// <param name="userId"> pass id of the user for details </param>
        /// <returns>List of all Users depending on 'Id' Value</returns>
        /// <response code="302">Users Data</response>
        /// <response code="404">No Data Found</response>
        [HttpGet]
        [Authorize]
        [Route("GetUserById/{userId}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserById(string userId)
        {
            var user = ISecurityServices.GetUserById(userId);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        /// <summary>
        /// Getting user by role id including User Name
        /// </summary>
        /// <param name="roleId"> pass role id of the user for details </param>
        /// <returns>List of all Users depending on 'Id' Value</returns>
        /// <response code="302">Users Data</response>
        /// <response code="404">No Data Found</response>
        [HttpGet]
        [Authorize]
        [Route("GetUserByRoleId/{roleId}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserByRoleId(int roleId)
        {
            var user = ISecurityServices.GetUserByRoleId(roleId);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        /// <summary>
        /// Send Verification Code to user's email
        /// </summary>
        /// <param name="userEmail"> Email that needs to be set verified </param>
        /// <returns>a flag to confirm user that email is sent successfully</returns>
        /// <response code="200">Verification code send successfully </response>
        /// <response code="400">Error while sending verification code to user.</response>
        [HttpGet]
        [Authorize]
        [Route("SendVerificationCode/{userEmail}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SendVerificationCode(string userEmail)
        {
            try
            {
                var isVerificationCodeSent = ISecurityServices.SendVerificationEmail(userEmail);
                if (isVerificationCodeSent)
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending verification code.");
            }

            return Ok(false);
        }

        /// <summary>
        /// Send temporary password to user's email
        /// </summary>
        /// <param name="userEmail"> Email of a user need to reset for temporary password </param>
        /// <returns>a flag to confirm user that temporary password's email is sent successfully</returns>
        /// <response code="200">temporary password send successfully </response>
        /// <response code="400">Error while sending temporary password to to user.</response>
        [HttpGet]
        [AllowAnonymous]
        [Route("ForgetPassword/{userEmail}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ForgetPassword(string userEmail)
        {
            try
            {
                var isVerificationCodeSent = ISecurityServices.ForgetPassword(userEmail);
                if (isVerificationCodeSent)
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending forget password.");
            }

            return Ok(false);
        }

        /// <summary>
        /// To Check if User Exists
        /// </summary>
        /// <param name="userEmail"> User Email Address </param>
        /// <returns>return 302 if user exists and 404 if user not exists</returns>
        /// <response code="302">User Already Exists </response>
        /// <response code="404">User Doesn't Exists </response>
        [HttpGet]
        [AllowAnonymous]
        [Route("IsUserExists/{userEmail}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult IsUserExists(string userEmail)
        {
            var isUserExists = ISecurityServices.IsUserEmailExists(userEmail);
            if (isUserExists)
            {
                return Ok(true);
            }

            return Ok(false);
        }

        /// <summary>
        /// To Check if Mosque Exists
        /// </summary>
        /// <param name="userName"> User Name </param>
        /// <returns>return 302 if mosque exists and 404 if mosque not exists</returns>
        /// <response code="302">Mosque Already Exists </response>
        /// <response code="404">Mosque Doesn't Exists </response>
        [HttpGet]
        [AllowAnonymous]
        [Route("IsUserNameExists/{userName}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult IsUserNameExists(string userName)
        {
            var isMosqueExists = ISecurityServices.IsUserNameExists(userName);
            if (isMosqueExists)
            {
                return Ok(true);
            }

            return Ok(false);
        }

        /// <summary>
        /// Verify User by passing verification code and Email sent in email.
        /// </summary>
        /// <param name="user"> Set Uemail and VerificationCode </param>
        /// <returns>a flag to confirm user that email is verified</returns>
        /// <response code="200">User Verified Successfully </response>
        /// <response code="400">Error while verifying user.</response>
        [HttpPut]
        [Authorize]
        [Route("VerifyUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult VerifyUser([FromBody] UserViewModel user)
        {
            try
            {
                var isUserVerified = ISecurityServices.VerifyUser(user.Email, user.VerificationCode);
                if (isUserVerified)
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while verifying user.");
            }

            return Ok(false);
        }

        /// <summary>
        /// Change User Password
        /// </summary>
        /// <param name="userModel"> Set Uid and Upassword </param>
        /// <returns>a flag to confirm user that password is changed or not - 'true' if password is successfully changed.</returns>
        /// <response code="200">Password changed successfully </response>
        /// <response code="400">Error while changing password.</response>
        [HttpPut]
        [Authorize]
        [Route("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ChangePassword([FromBody] User userModel)
        {
            try
            {
                var isPasswordChanged = ISecurityServices.ChangePassword(userModel.IdUser, userModel.Password);
                if (isPasswordChanged)
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password.");
            }

            return Ok(false);
        }

        /// <summary>
        /// Update User Account Details
        /// </summary>
        /// <param name="userModel"> Id of the user whose account needs a change in records </param>
        /// <returns>a flag to confirm that user account is updated or not - 'true' if user is updated.</returns>
        /// <response code="200">Account updated successfully </response>
        /// <response code="400">Error while updating your account.</response>
        [HttpPut]
        [Authorize]
        [Route("UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUser([FromBody] User userModel)
        {
            try
            {
                var updatedUser = ISecurityServices.UpdateUserDetails(userModel);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user.");
            }

            return Ok(false);
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="userModel"> Set Email, Password, UserName, FirstName, LastName </param>
        /// <returns>a JWT token for logged in user</returns>
        /// <response code="200">User added successfully </response>
        /// <response code="400">Error while Adding User</response>
        /// <response code="500">Internal Server Error or Error while handshaking with database</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RegisterUser([FromBody] UserViewModel userModel)
        {
            try
            {
                var user = ISecurityServices.CreateUser(userModel);                
                if (user != null)
                {
                    var userToken = ISecurityServices.GenerateJSONWebToken(user);
                    return Ok(new { token = userToken });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user.");
            }

            return BadRequest("Adding user failed. Please try again.");
        }

        /// <summary>
        /// Authenticate and Login User
        /// </summary>
        /// <param name="userModel"> Credentials to authenticate user from database, pass only uemail and upassword </param>
        /// <returns>a JWT token for logged in user</returns>
        /// <response code="200">User successfully logged In</response>
        /// <response code="400">Error while processing the request.</response>
        /// <response code="401">Unauthorized Access.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("LoginUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult LoginUser([FromBody] UserViewModel userModel)
        {
            try
            {
                var user = ISecurityServices.AuthenticateUser(userModel);
                if (user != null)
                {
                    var userToken = ISecurityServices.GenerateJSONWebToken(user);
                    return Ok(new { token = userToken });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while logging user.");
            }

            return Unauthorized();
        }
    }
}
