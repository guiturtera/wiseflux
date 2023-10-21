using System.ComponentModel.DataAnnotations;

namespace Wiseflux.Models
{
    /// <summary>
    /// Used for return of the login
    /// </summary>
    public class UserTokenModel
    {
        /// <summary>
        /// The current login of the user without its key.
        /// </summary>
        [Required]
        public User User { get; set; }

        /// <summary>
        /// Token of a user
        /// </summary>
        [Required]
        public string Token { get; set; }

        /// <summary>
        /// Datetime of expiration of the user token
        /// </summary>
        [Required]
        public DateTime TokenExpiryTime { get; set; }

        /// <summary>
        /// Refresh token for the user. Used to login the user in the page if default token is expired.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Tempo de expiração do refresh token
        /// </summary>
        [Required]
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
