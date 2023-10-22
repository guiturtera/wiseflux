
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
        private readonly AuthService _authService;

        public AuthController(ApplicationDbContext db, ITokenService tokenService, AuthService authService)
        {
            _db = db;
            _tokenService = tokenService;
            _authService = authService;
        }

        private int hoursTokenExpiration = 2;
        private int hoursRefreshTokenExpiration = 24;

        /// <summary>
        /// Login into the application. Returns the user, token and refresh token with their metadata.
        /// </summary>
        /// <param name="login">The specified CPF/Password of your user.</param>
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<UserTokenModel>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] LoginModel login)
        {
            var result = await _authService.Authenticate(login);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserTokenModel))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        public async Task<ActionResult<dynamic>> Refresh([FromBody] RefreshTokenModel refreshTokenModel)
        {
            var result = await _authService.Refresh(refreshTokenModel);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        [HttpPost("revoke")]
        [ProducesResponseType((int)System.Net.HttpStatusCode.NoContent, Type = typeof(void))]
        [ProducesResponseType((int)System.Net.HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult<dynamic>> Revoke()
        {
            var result = await _authService.Revoke(User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }
    }
}
