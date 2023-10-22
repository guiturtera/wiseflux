using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Helpers;
using Wiseflux.Interfaces;
using Wiseflux.Models;
using Wiseflux.Security;

namespace Wiseflux.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITokenService _tokenService;

        private int hoursTokenExpiration = 2;
        private int hoursRefreshTokenExpiration = 24;

        public AuthService(ApplicationDbContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<ServiceResponse<UserTokenModel>> Authenticate(LoginModel login)
        {
            HttpResponseMessage errorResponse;
            var user = _db.Users.Find(login.Email);
            if (user == null || !new UserSecurity().CheckPassword(login.Password, user.Password))
                return new ServiceResponse<UserTokenModel>(HttpStatusCode.NotFound, "Username or password invalid!", null);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(hoursRefreshTokenExpiration);
            var generatedToken = generateNewToken(user);
            user.RefreshToken = generatedToken.RefreshToken;
            
            _db.SaveChanges();

            user.Password = "";
            return new ServiceResponse<UserTokenModel>(HttpStatusCode.OK, null, generatedToken);
        }

        public async Task<ServiceResponse<UserTokenModel>> Refresh(RefreshTokenModel refreshTokenModel)
        {
            string accessToken = refreshTokenModel.Token;
            string refreshToken = refreshTokenModel.RefreshToken;

            var userClaims = _tokenService.GetUserFromExpiredToken(accessToken);

            if (userClaims == null) 
                return new ServiceResponse<UserTokenModel>(HttpStatusCode.Forbidden, "Tokens inválidos", null);

            var email = ClaimsHelper.GetUserEmail(userClaims);
            var user = _db.Users.Find(email);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new ServiceResponse<UserTokenModel>(HttpStatusCode.Forbidden, "Tokens inválidos", null);

            var generatedToken = generateNewToken(user);
            user.RefreshToken = generatedToken.RefreshToken;
            _db.SaveChanges();

            return new ServiceResponse<UserTokenModel>(HttpStatusCode.OK, null, generatedToken);
        }

        public async Task<ServiceResponse<object>> Revoke(ClaimsPrincipal currentUser)
        {
            var email = ClaimsHelper.GetUserEmail(currentUser);
            var user = _db.Users.Find(email);

            if (user == null) 
                return new ServiceResponse<object>(HttpStatusCode.NotFound, "User not found", null);

            user.RefreshToken = null;

            _db.SaveChanges();
            return new ServiceResponse<object>(HttpStatusCode.NoContent, "Success", null);
        }

        private UserTokenModel generateNewToken(User user)
        {
            var tokenExpiryDate = DateTime.UtcNow.AddHours(hoursTokenExpiration);
            var newAccessToken = _tokenService.GenerateToken(user, tokenExpiryDate);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new UserTokenModel
            {
                User = user,
                Token = newAccessToken,
                TokenExpiryTime = tokenExpiryDate,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };
        }
    }
}
