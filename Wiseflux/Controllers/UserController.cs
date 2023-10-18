using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wiseflux.Data;
using Wiseflux.Models;
using Wiseflux.Security;

namespace Wiseflux.Controllers
{
    [Route("api/[controller]")]
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
        [Authorize]
        public ActionResult GetCurrentUserInfo()
        {
            var claims = User.Identities.First().Claims.ToList();
            string cpf = claims?.FirstOrDefault(x => x.Type.Equals("cpf", StringComparison.OrdinalIgnoreCase)).Value;
            var user = _db.Users.Find(cpf);
            return new JsonResult(user);
        }

        /// <summary>
        /// Returns a list of all Users from the DB.
        /// </summary>
        [HttpGet("admin/get/all")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(IEnumerable<User>))]
        [Authorize(Roles = "admin")]
        public ActionResult GetUsersInfo()
        {
            var users = _db.Users.ToList();
            return new JsonResult(users);
        }

        /// <summary>
        /// Returns a specific user by his CPF.
        /// </summary>
        /// <param name="cpf">CPF of the user to search. Format it `^[0-9]{11}`</param>
        [HttpGet("admin/get/{cpf}")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(User))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.NotFound, Type = typeof(HttpResponseMessage))]
        [Authorize(Roles = "admin")]
        public ActionResult GetUserInfo(string cpf)
        {
            // If no user is found, will let it raise 204 error.
            // Depending, Could add cpf validation
            User user = _db.Users.Find(cpf);
            if (user == null)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"User with CPF `{cpf}` not found!"),
                    ReasonPhrase = "User not found!"
                };
                return new JsonResult(response);
            }

            return new JsonResult(user);
        }

        /// <summary>
        /// Deletes a user from DB. If user doesn't exist, will return 400 (BadRequest).
        /// </summary>
        /// <param name="cpf">CPF of the user to delete from DB</param>
        [HttpDelete("admin/delete")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(string cpf)
        {
            User userToDelete;
            bool userExists = UserExists(cpf, out userToDelete);

            HttpResponseMessage errorResponse;
            if (!ValidateUserExists(cpf, out userToDelete, out errorResponse))
                return new JsonResult(errorResponse);

            _db.Users.Remove(userToDelete);
            _db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Adds a new user to the DB.
        /// </summary>
        /// <param name="newUser">Json of the new user to add. See User Schema for more info</param>
        [HttpPost("admin/add")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize(Roles = "admin")]
        public ActionResult AddUser([FromBody] User newUser)
        {
            // Chose to add validation here. Could add in Schemas 
            HttpResponseMessage errorResponse;
            if (!ValidUser(newUser, out errorResponse) || !ValidateUserNotExists(newUser.CPF, out errorResponse))
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
        [HttpPut("admin/edit")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Forbidden, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.NotFound, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.BadRequest, Type = typeof(HttpResponseMessage))]
        [Authorize(Roles = "admin")]
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
                    Content = new StringContent($"User with {user.CPF} doesn't exist. Try adding before editing."),
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

        private bool UserExists(string cpf, out User user)
        {
            user = _db.Users.Find(cpf);
            return user != null;
        }

        private bool ValidateUserExists(string cpf, out User user, out HttpResponseMessage errorResponse)
        {
            user = null;
            errorResponse = null;
            bool userExists = UserExists(cpf, out user);

            if (!userExists)
            {
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"User with CPF `{cpf}` does not exists"),
                    ReasonPhrase = "User not found!"
                };
                return false;
            }

            return userExists;
        }

        private bool ValidateUserNotExists(string cpf, out HttpResponseMessage errorResponse)
        {
            errorResponse = null;
            bool valid = !UserExists(cpf, out User user);
            if (!valid)
            {
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent($"User with CPF `{cpf}` already exists"),
                    ReasonPhrase = "User already exists"
                };
            }

            return valid;
        }
    }
}