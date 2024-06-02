using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailServer.Model
{
    public class EmailMessage
    {
        public MailboxAddress To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EmailMessage(string name,string address,string subject,string content)
        {
            To = new MailboxAddress(name,address);
            Subject = subject;
            Content = content;
        }
    }
}
