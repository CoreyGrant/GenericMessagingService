using GenericMessagingService.Services.StorageService;
using GenericMessagingService.Services.Utils;
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
    internal class FileEmailService : IEmailService
    {
        private readonly FolderSettings settings;
        private readonly IStorageService storageService;

        public FileEmailService(FolderSettings settings, IStorageService storageService)
        {
            this.settings = settings;
            this.storageService = storageService;
        }
        public async Task SendEmailAsync(string subject, string body, IEnumerable<string> to, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".msg";
            using (var email = new MsgKit.Email(
                new MsgKit.Sender(from, from),
                new MsgKit.Representing(from, from),
                subject))
            {
                email.Subject = subject;
                email.BodyHtml = body;
                foreach (var toAddress in to)
                {
                    if(email.Recipients.Count() == 0)
                    {
                        email.Recipients.AddTo(toAddress);
                    }
                    else
                    {
                        email.Recipients.AddBcc(toAddress);
                    }
                }
                using(var stream = new MemoryStream())
                {
                    email.Save(stream);
                    await storageService.StoreFile(stream, settings.FolderPath, fileName);
                }
                
            }
        }

        public async Task SendEmailAsync(Dictionary<string, (string, string)> toSubjectBody, string from)
        {
            foreach(var (to, (subject, body)) in toSubjectBody)
            {
                var fileName = Guid.NewGuid().ToString() + ".msg";
                using (var email = new MsgKit.Email(
                new MsgKit.Sender(from, from),
                new MsgKit.Representing(from, from),
                subject))
                {
                    email.Subject = subject;
                    email.BodyHtml = body;
                    email.Recipients.AddTo(to);
                    using (var stream = new MemoryStream())
                    {
                        email.Save(stream);
                        await storageService.StoreFile(stream, settings.FolderPath, fileName);
                    }
                }
            }
            
        }
    }
}
