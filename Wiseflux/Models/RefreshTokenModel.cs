using System.ComponentModel.DataAnnotations;

namespace Wiseflux.Models
{
    public class RefreshTokenModel
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        public string? RefreshToken { get; set; }
    }
}
