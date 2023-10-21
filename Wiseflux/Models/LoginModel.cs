using System.ComponentModel.DataAnnotations;

namespace Wiseflux.Models
{
    /// <summary>
    /// Schema for realizing login.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Email of an existing user.
        /// </summary>
        /// <example>foo@gmail.com</example>
        [Required]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        /// <summary>
        /// Password of an existing user.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
