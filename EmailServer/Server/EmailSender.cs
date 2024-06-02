using EmailServer.Model;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailServer.Server
{
    public class EmailSender : IEmailSender
    {
        private readonly EmaliSendConfig _emaliSendConfig;

        public EmailSender(IOptions<EmaliSendConfig> emaliSendConfig)
        {
            _emaliSendConfig = emaliSendConfig.Value;
        }
        public void SendEmailAsync(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emaliSendConfig.Name,_emaliSendConfig.Address));
            emailMessage.To.Add(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new BodyBuilder { HtmlBody= $"<h1>{message.Content}</h1>" }.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                 client.Connect(_emaliSendConfig.SmtpServer, _emaliSendConfig.Port,true);
                 client.Authenticate(_emaliSendConfig.UserName, _emaliSendConfig.Password);
                 client.Send(emailMessage);
            }
            catch (Exception ex)
            {
                // 处理异常，例如记录日志
                throw;
            }
            finally
            {
                client.Disconnect(true);
            }
        }

    }
}
