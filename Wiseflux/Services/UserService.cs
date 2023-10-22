using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Helpers;
using Wiseflux.Models;
using Wiseflux.Security;

namespace Wiseflux.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<User>> GetCurrentUserInfo(ClaimsPrincipal userClaim)
        {
            User currentUser;
            if (!UserExistsByClaim(userClaim, out currentUser)) 
                return new ServiceResponse<User>(HttpStatusCode.NotFound, "User not found", null);

            currentUser.Password = "";

            return new ServiceResponse<User>(HttpStatusCode.OK, "Success", currentUser);
        }

        public async Task<ServiceResponse<object>> DeleteUser(ClaimsPrincipal userClaim)
        {
            User userToDelete;
            if (!UserExistsByClaim(userClaim, out userToDelete))
                return new ServiceResponse<object>(HttpStatusCode.NotFound, "User not found", null);

            _db.Users.Remove(userToDelete);
            _db.SaveChanges();

            return new ServiceResponse<object>(HttpStatusCode.OK, "Success", null);
        }

        public async Task<ServiceResponse<object>> AddUser(User newUser)
        {
            User existingUser;
            if (UserExistsByEmail(newUser.Email, out existingUser))
                return new ServiceResponse<object>(HttpStatusCode.Conflict, "User already exists", null);

            newUser.Password = new UserSecurity().EncryptPassword(newUser.Password);

            _db.Users.Add(newUser);
            _db.SaveChanges();

            return new ServiceResponse<object>(HttpStatusCode.OK, "Success", null);
        }

        public async Task<ServiceResponse<object>> EditUser(User editedUser, ClaimsPrincipal userClaim)
        {
            if (editedUser.Email != ClaimsHelper.GetUserEmail(userClaim))
                return new ServiceResponse<object>(HttpStatusCode.BadRequest, "You can not change the email address of the account!", null);

            editedUser.Password = new UserSecurity().EncryptPassword(editedUser.Password);
            _db.Users.Update(editedUser);
            try
            {
                _db.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return new ServiceResponse<object>(HttpStatusCode.BadRequest, "User not found", null);
            }

            return new ServiceResponse<object>(HttpStatusCode.OK, "Success", null);
        }

        private bool UserExistsByClaim(ClaimsPrincipal userClaim, out User user)
        {
            string email = ClaimsHelper.GetUserEmail(userClaim);
            return UserExistsByEmail(email, out user);  
        }

        private bool UserExistsByEmail(string email, out User user)
        {
            user = _db.Users.Find(email);
            return user != null;
        }
    }
}
