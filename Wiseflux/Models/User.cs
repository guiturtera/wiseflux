using System.ComponentModel.DataAnnotations;

namespace Wiseflux.Models
{
    /// <summary>
    /// Available Roles for a User. It specifies which methods they have access
    /// </summary>
    public enum EnumUserRoles
    {
        /// <summary>
        /// Admin have access to all methods.
        /// </summary>
        Admin = 0,
        /// <summary>
        /// DefaultUser only have access to their data.
        /// </summary>
        DefaultUser = 1
    }

    /// <summary>
    /// Are used to store data about a person
    /// </summary>
    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        ///
        /// </summary>
        [Required]
        [MinLength(10)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Admin = 0
        /// DefaultUser = 1
        /// Check EnumUserRoles for more info
        /// </summary>
        [Required]
        [EnumDataType(typeof(EnumUserRoles), ErrorMessage = "Please verify if you entered a valid role in `EnumUserRoles` Scheme!")]
        public EnumUserRoles Role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
