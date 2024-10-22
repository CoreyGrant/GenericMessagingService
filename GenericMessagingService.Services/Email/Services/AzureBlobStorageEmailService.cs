﻿using GenericMessagingService.Services.Utils;
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
    internal class AzureBlobStorageEmailService : IEmailService
    {
        private readonly AzureBlobStorageSettings settings;
        private readonly IAzureBlobStorageManager azureBlobStorageManager;

        public AzureBlobStorageEmailService(AzureBlobStorageSettings settings, IAzureBlobStorageManager azureBlobStorageManager)
        {
            this.settings = settings;
            this.azureBlobStorageManager = azureBlobStorageManager;
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
                    await this.azureBlobStorageManager.StoreFile(stream, settings.FolderPath, fileName);
                }
                
            }
        }

        public async Task SendEmailAsync(Dictionary<string, (string, string)> toSubjectBody, string from)
        {
            var tasks = new List<Task>();
            foreach(var (to, (subject, body)) in toSubjectBody)
            {
                var task = Task.Run(async () =>
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
                            await this.azureBlobStorageManager.StoreFile(stream, settings.FolderPath, fileName);
                        }
                    }
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }
    }
}
