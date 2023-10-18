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
        /// The CPF of the User. It is a PK. Format must be `^[0-9]{11}`. 
        /// Not added math verification.
        /// </summary>
        [Key]
        [Required]
        [RegularExpression(@"^[0-9]{11}")]
        public string CPF { get; set; }
        /// <summary>
        /// The name of the user.
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Birth date of the user.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Admin = 0
        /// DefaultUser = 1
        /// Check EnumUserRoles for more info
        /// </summary>
        [Required]
        [EnumDataType(typeof(EnumUserRoles), ErrorMessage = "Please verify if you entered a valid role in `EnumUserRoles` Scheme!")]
        public EnumUserRoles Role { get; set; }
        /// <summary>
        /// Password of the user. Will be stored as HASH in the DB.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
