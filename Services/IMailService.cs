using System;
using JwtAuth.Models;
namespace JwtAuth.Services
{
    public interface IMailService
    {
        Task SendMailAsync(User user,string subject,string body);
    }
}