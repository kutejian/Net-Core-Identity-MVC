using EmailServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailServer.Server
{
    public interface IEmailSender
    {
        void SendEmailAsync(EmailMessage emailMessage);
    }
}
