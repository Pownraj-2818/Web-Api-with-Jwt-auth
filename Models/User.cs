using System.ComponentModel.DataAnnotations;


namespace JwtAuth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? username { get; set; }
        public byte[]? passwordHash { get; set; }

        public byte[]? passwordSalt { get; set; }
        public string? email { get; set; }
        public string? role { get; set; }
        public string? verficationCode {get;set;}
        public string? resetToken {get;set;}
        public DateTime? verifiedat {get;set;}
        public string? RefreshToken { get; set; } = String.Empty;

        public DateTime DateCreated { get; set; }

        public DateTime TokenExpires { get; set; }
    }
}