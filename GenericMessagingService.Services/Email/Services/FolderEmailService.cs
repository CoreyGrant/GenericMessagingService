using GenericMessagingService.Types.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email.Services
{
    internal class FolderEmailService : IEmailService
    {
        private readonly FolderSettings settings;

        public FolderEmailService(FolderSettings settings)
        {
            this.settings = settings;
        }
        public async Task SendEmailAsync(string subject, string body, IEnumerable<string> to, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".msg";
            using (var email = new MsgKit.Email(
                new MsgKit.Sender(from, ""),
                new MsgKit.Representing("", ""),
                subject))
            {
                email.Subject = subject;
                email.BodyHtml = body;
                foreach (var toAddress in to)
                {
                    email.Recipients.AddTo(toAddress);
                }
                email.Save(fileName);
            }
        }
    }
}
