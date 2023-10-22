using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Models;
using Wiseflux.Security;

namespace Wiseflux.Controllers
{ 
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns info about the current user
        /// </summary>
        [HttpGet("info")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserInfo()
        {
            var email = GetUserEmailFromToken();
            var user = _db.Users.Find(email);
            if (user == null) return new JsonResult(DefaultError("User not found.", HttpStatusCode.NotFound));

            user.Password = "";

            return new JsonResult(user);
        }

        /// <summary>
        /// Deletes a user from DB. If user doesn't exist, will return 400 (BadRequest).
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize]
        public async Task<ActionResult> DeleteUser()
        {
            string email = GetUserEmailFromToken();
            
            User userToDelete;
            HttpResponseMessage errorResponse;
            if (!ValidateUserExists(email, out userToDelete, out errorResponse))
                return new JsonResult(errorResponse);

            _db.Users.Remove(userToDelete);
            _db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Adds a new user to the DB.
        /// </summary>
        /// <param name="newUser">Json of the new user to add. See User Schema for more info</param>
        [HttpPost("add")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [AllowAnonymous]
        public async Task<ActionResult> AddUser([FromBody] User newUser)
        {
            // Chose to add validation here. Could add in Schemas 
            HttpResponseMessage errorResponse;
            if (!ValidUser(newUser, out errorResponse) || !ValidateUserNotExists(newUser.Email, out errorResponse))
                return new JsonResult(errorResponse);

            newUser.Password = new UserSecurity().EncryptPassword(newUser.Password);

            _db.Users.Add(newUser);
            _db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Edit a specific user from the DB.
        /// </summary>
        /// <param name="user">Data of the user to edit</param>
        [HttpPut("edit")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize]
        public async Task<ActionResult> EditUser([FromBody] User user)
        {
            HttpResponseMessage errorResponse;
            if (!ValidUser(user, out errorResponse))
                return new JsonResult(errorResponse);
            if (user.Email != GetUserEmailFromToken())
                return new JsonResult(DefaultError("You can not change the email address of the account!", HttpStatusCode.BadRequest));

            user.Password = new UserSecurity().EncryptPassword(user.Password);
            _db.Users.Update(user);
            try
            {
                _db.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return new JsonResult(DefaultError("User not found.", HttpStatusCode.NotFound));
            }

            return Ok();
        }

        private bool ValidUser(User user, out HttpResponseMessage errorResponse)
        {
            errorResponse = null;
            if (!ModelState.IsValid)
            {
                var allErrors = (string[])ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
                errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(String.Join(',', allErrors)),
                    ReasonPhrase = "Invalid parameters."
                };
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }

            return true;
        }

        private bool UserExists(string email, out User user)
        {
            user = _db.Users.Find(email);
            return user != null;
        }

        private bool ValidateUserExists(string email, out User user, out HttpResponseMessage errorResponse)
        {
            user = null;
            errorResponse = null;
            bool userExists = UserExists(email, out user);

            if (!userExists)
            {
                errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"User with EMAIL `{email}` does not exists"),
                    ReasonPhrase = "User not found!"
                };
                return false;
            }

            return userExists;
        }

        private bool ValidateUserNotExists(string email, out HttpResponseMessage errorResponse)
        {
            errorResponse = null;
            bool userExists = UserExists(email, out User user);
            
            if (userExists) 
                errorResponse = DefaultError($"User with email `{email}` already exists", HttpStatusCode.Conflict);

            return !userExists;
        }

        private HttpResponseMessage DefaultError(string errorReason, HttpStatusCode httpStatusCode)
        {
            var errorResponse = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(errorReason),
                ReasonPhrase = errorReason
            };
            Response.StatusCode = (int)(httpStatusCode);

            return errorResponse;
        }

        private string GetUserEmailFromToken()
        {
            List<Claim> claims = User.Identities.First().Claims.ToList();
            string? email = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).Value;

            return email;
        }
    }
}