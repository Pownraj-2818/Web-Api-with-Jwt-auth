
using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Models
{
    public class ResetPassword
    {
        [Required]
        public string? token { get; set; }
        [Required]
        public string? password { get; set; }
        [Compare("password")]
        public string? confirm_password { get; set; }
    }
}