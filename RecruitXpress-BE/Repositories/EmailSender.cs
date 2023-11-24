

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using NETCore.MailKit.Core;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;

namespace RecruitXpress_BE.Repositories
{
    public class EmailSender :IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
 

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _emailConfig.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailConfig.UserName, _emailConfig.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
