using GenericMessagingService.Types.Config;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email.Services
{
    internal class SendGridEmailService : IEmailService
    {
        private readonly SendGridSettings settings;
        private readonly ISendGridClient sendGridClient;

        public SendGridEmailService(SendGridSettings settings, ISendGridClient sendGridClient)
        {
            this.settings = settings;
            this.sendGridClient = sendGridClient;
        }

        public async Task SendEmailAsync(string subject, string body, IEnumerable<string> to, string from)
        {
            var msg = new SendGridMessage
            {
                Subject = subject,
                From = new EmailAddress(from),
                HtmlContent = body,
            };
            msg.AddTos(to.Select(x => new EmailAddress(x)).ToList());
            await sendGridClient.SendEmailAsync(msg);
        }

        public async Task SendEmailAsync(Dictionary<string, (string, string)> toSubjectBody, string from)
        {
            var tasks = new List<Task>();
            foreach(var (to, (subject, body)) in toSubjectBody)
            {
                var msg = new SendGridMessage
                {
                    Subject = subject,
                    From = new EmailAddress(from),
                    HtmlContent = body,
                };
                msg.AddTo(new EmailAddress(to));
                tasks.Add(sendGridClient.SendEmailAsync(msg));
            }
            await Task.WhenAll(tasks);
        }
    }
}
