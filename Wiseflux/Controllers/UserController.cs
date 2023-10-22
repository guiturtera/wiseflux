using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public ActionResult GetCurrentUserInfo()
        {
            var claims = User.Identities.First().Claims.ToList();
            string? email = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).Value;
            var user = _db.Users.Find(email);
            user.Password = "";

            return new JsonResult(user);
        }

        /// <summary>
        /// Deletes a user from DB. If user doesn't exist, will return 400 (BadRequest).
        /// </summary>
        /// <param name="email">EMAIL of the user to delete from DB</param>
        [HttpDelete("delete")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize]
        public ActionResult DeleteUser(string email)
        {
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
        [AllowAnonymous]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        public ActionResult AddUser([FromBody] User newUser)
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
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.NotFound, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize]
        public ActionResult EditUser([FromBody] User user)
        {
            HttpResponseMessage errorResponse;
            if (!ValidUser(user, out errorResponse))
                return new JsonResult(errorResponse);

            user.Password = new UserSecurity().EncryptPassword(user.Password);

            _db.Users.Update(user);
            try
            {
                _db.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"User with email '{user.Email}' doesn't exist. Try adding before editing."),
                    ReasonPhrase = "User don't exist"
                };
                return new JsonResult(errorResponse);
            }

            return Ok();
        }

        private bool ValidUser(User user, out HttpResponseMessage errorResponse)
        {
            errorResponse = null;
            if (!ModelState.IsValid)
            {
                var allErrors = (string[])ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage));
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(String.Join(',', allErrors)),
                    ReasonPhrase = "Invalid parameters."
                };
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
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
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
            bool valid = !UserExists(email, out User user);
            if (!valid)
            {
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent($"User with email `{email}` already exists"),
                    ReasonPhrase = "User already exists"
                };
            }

            return valid;
        }
    }
}