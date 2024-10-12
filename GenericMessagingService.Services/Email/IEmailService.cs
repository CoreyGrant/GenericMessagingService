using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email
{
    internal interface IEmailService
    {
        Task SendEmailAsync(string subject, string body, IEnumerable<string> to, string from);
    }
}
