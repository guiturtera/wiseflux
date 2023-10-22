using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Models;
using Wiseflux.Security;
using Wiseflux.Services;

namespace Wiseflux.Controllers
{ 
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Returns info about the current user
        /// </summary>
        [HttpGet("info")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<User>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserInfo()
        {
            var result = await _userService.GetCurrentUserInfo(User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Deletes a user from DB. If user doesn't exist, will return 400 (BadRequest).
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ServiceResponse<object>))]
        [Authorize]
        public async Task<ActionResult> DeleteUser()
        {
            var result = await _userService.DeleteUser(User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Adds a new user to the DB.
        /// </summary>
        /// <param name="newUser">Json of the new user to add. See User Schema for more info</param>
        [HttpPost("add")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ServiceResponse<object>))]
        [AllowAnonymous]
        public async Task<ActionResult> AddUser([FromBody] User newUser)
        {
            var result = await _userService.AddUser(newUser, ModelState);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Edit a specific user from the DB.
        /// </summary>
        /// <param name="user">Data of the user to edit</param>
        [HttpPut("edit")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ServiceResponse<object>))]
        [Authorize]
        public async Task<ActionResult> EditUser([FromBody] User user)
        {
            var result = await _userService.EditUser(user, User, ModelState);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }
    }
}