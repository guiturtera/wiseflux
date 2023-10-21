using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wiseflux.Models
{
    /// <summary>
    /// Available Roles for a User. It specifies which methods they have access
    /// </summary>
    public enum EnumUserRoles
    {
        /// <summary>
        /// Default users have access to specific features
        /// </summary>
        Default = 0,
        /// <summary>
        /// Vip users have access to all the platform features
        /// </summary>
        Vip = 1
    }

    /// <summary>
    /// Are used to store data about a person
    /// </summary>
    public class User
    {
        /// <summary>
        /// Email do usuário
        /// </summary>
        /// <example>foo@gmail.com</example>
        [Key]
        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
        public string Email { get; set; }
        
        /// <summary>
        /// Telefone do usuário
        /// </summary>
        [Required]
        [MinLength(10)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        [Required]
        [MinLength(4)]
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
        /// Senha do usuário (encriptada)
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }

        /// <summary>
        /// Refresh token atrelado ao usuário
        /// </summary>
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Data de expiração do refresh token atrelado ao usuário
        /// </summary>
        [JsonIgnore]
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
