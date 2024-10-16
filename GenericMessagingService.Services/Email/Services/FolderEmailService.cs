﻿using GenericMessagingService.Types.Config;
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
                    if(email.Recipients.Count() == 0)
                    {
                        email.Recipients.AddTo(toAddress);
                    }
                    else
                    {
                        email.Recipients.AddBcc(toAddress);
                    }
                    
                }
                email.Save(fileName);
            }
        }

        public async Task SendEmailAsync(Dictionary<string, (string, string)> toSubjectBody, string from)
        {
            var tasks = new List<Task>();
            foreach(var (to, (subject, body)) in toSubjectBody)
            {
                var task = Task.Run(() =>
                {
                    var fileName = Guid.NewGuid().ToString() + ".msg";
                    using (var email = new MsgKit.Email(
                    new MsgKit.Sender(from, ""),
                    new MsgKit.Representing("", ""),
                    subject))
                    {
                        email.Subject = subject;
                        email.BodyHtml = body;
                        email.Recipients.AddTo(to);
                        email.Save(fileName);
                    }
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }
    }
}