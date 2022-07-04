using MailKit.Net.Smtp;
using JwtAuth.Models;
using JWTAUTH.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace JwtAuth.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings=mailSettings.Value;
        }
        public async Task SendMailAsync(User user,string subject,string body)
        {
           
            var email = new MimeMessage();
            email.Sender=MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(user.email));
            email.Subject=subject;
           
            var builder = new BodyBuilder();

            // if(mailRequest.Attachments!=null)
            //     {
            //         byte[] fileBytes;

            //         foreach(var files in mailRequest.Attachments)
            //         {
            //             if(files.Length>0)
            //             {
            //                 using (var ms=new MemoryStream())
            //                 {
            //                     files.CopyTo(ms);
            //                     fileBytes=ms.ToArray();
            //                 }
            //                 builder.Attachments.Add(files.FileName,fileBytes,ContentType.Parse(files.ContentType));
            //             }
            //         }
            //     }
                
                builder.HtmlBody=body ;
                email.Body=builder.ToMessageBody();
                using var smtp=new SmtpClient();
                smtp.Connect(_mailSettings.Host,_mailSettings.Port,SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail,_mailSettings.Password);
                 
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

        }
    }
}