
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Interfaces;
using Wiseflux.Models;
using Wiseflux.Security;
using Wiseflux.Services;

namespace Wiseflux.Controllers
{
    [Route($"api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ITokenService _tokenService;

        public AuthController(ApplicationDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        private int hoursTokenExpiration = 2;
        private int hoursRefreshTokenExpiration = 24;

        /// <summary>
        /// Login into the application. This login is uses Bearer Token.
        /// </summary>
        /// <param name="login">The specified CPF/Password of your user.</param>
        [HttpPost("login")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(HttpResponseMessage))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.NotFound, Type = typeof(UserTokenModel))]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] LoginModel login)
        {
            HttpResponseMessage errorResponse;
            var user = _db.Users.Find(login.Email);
            if (user == null || !new UserSecurity().CheckPassword(login.Password, user.Password))
            {
                errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent($"Username or password invalid!"),
                    ReasonPhrase = "Wrong username/password"
                };
                Response.StatusCode = 403;
                return errorResponse;
            }

            var tokenExpiryDate = DateTime.UtcNow.AddHours(hoursTokenExpiration);
            var token = _tokenService.GenerateToken(user, tokenExpiryDate);
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(hoursRefreshTokenExpiration);

            _db.SaveChanges();
            
            user.Password = "";
            var userTokenModel = new UserTokenModel
            {
                User = user,
                Token = token,
                TokenExpiryTime = tokenExpiryDate,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };
            return new JsonResult(userTokenModel);
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.OK, Type = typeof(UserTokenModel))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        public async Task<ActionResult<dynamic>> Refresh(RefreshTokenModel refreshTokenModel)
        {
            string accessToken = refreshTokenModel.Token;
            string refreshToken = refreshTokenModel.RefreshToken;

            var userClaims = _tokenService.GetUserFromExpiredToken(accessToken);

            if (userClaims == null) 
            {
                return DefaultError("Tokens inválidos.", System.Net.HttpStatusCode.Forbidden);
            }

            var email = userClaims.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).Value;

            var user = _db.Users.Find(email);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return DefaultError("Tokens inválidos.", HttpStatusCode.Forbidden);

            var tokenExpiryDate = DateTime.UtcNow.AddHours(hoursTokenExpiration);
            var newAccessToken = _tokenService.GenerateToken(user, tokenExpiryDate);
            user.RefreshToken = _tokenService.GenerateRefreshToken(); 

            _db.SaveChanges();
            return Ok(new UserTokenModel()
            {
                User = user,
                Token = newAccessToken,
                TokenExpiryTime = tokenExpiryDate,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            });
        }

        [HttpPost("revoke")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult<dynamic>> Revoke()
        {
            var email = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).Value;
            var user = _db.Users.Find(email);

            if (user == null) return DefaultError("Usuário não existe mais", HttpStatusCode.BadRequest);

            user.RefreshToken = null;
            _db.SaveChanges();
            return NoContent();
        }

        private HttpResponseMessage DefaultError(string errorReason, HttpStatusCode httpStatusCode)
        {
            var errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
            {
                Content = new StringContent(errorReason),
                ReasonPhrase = errorReason
            };
            Response.StatusCode = (int)(httpStatusCode);

            return errorResponse;
        }

    }
}
