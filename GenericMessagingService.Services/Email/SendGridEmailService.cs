using GenericMessagingService.Types.Config;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email
{
    internal class SendGridEmailService : IEmailService
    {
        private readonly SendGridSettings settings;

        public SendGridEmailService(SendGridSettings settings) 
        {
            this.settings = settings;
        }

        public async Task SendEmailAsync(string subject, string body, IEnumerable<string> to, string from)
        {
            var sendGridClient = new SendGridClient(new SendGridClientOptions
            {
                ApiKey = this.settings.ApiKey
            });
            var msg = new SendGridMessage
            {
                Subject = subject,
                From = new EmailAddress(from),
                HtmlContent = body,
            };
            msg.AddTos(to.Select(x => new EmailAddress(x)).ToList());
            await sendGridClient.SendEmailAsync(msg);
        }
    }
}
