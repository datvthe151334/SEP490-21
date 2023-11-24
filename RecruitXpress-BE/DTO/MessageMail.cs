using MailKit.Net.Smtp;
using MimeKit;
namespace RecruitXpress_BE.DTO
{
    public class MessageMail
    {
  
            public List<MimeKit.MailboxAddress> To { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
            public MessageMail(IEnumerable<string> to, string subject, string content)
            {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x,string.Empty)));
            Subject = subject;
            Content = content;
        }
    }
 
    }
