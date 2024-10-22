﻿using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Sms.Services
{
    internal class FolderSmsService : ISmsService
    {
        private readonly FolderSettings settings;
        private readonly IFileManager fileManager;

        public FolderSmsService(FolderSettings settings, IFileManager fileManager)
        {
            this.settings = settings;
            this.fileManager = fileManager;
        }

        public async Task SendSms(string message, IEnumerable<string> to, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            var filePath = Path.Join(settings.FolderPath, fileName);
            var fileText = @$"From: {from}
To: {string.Join(", ", to)}
Message: {message}";
            await fileManager.WriteFileAsync(filePath, fileText);
        }

        public async Task SendSms(Dictionary<string, string> toMessages, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            var filePath = Path.Join(settings.FolderPath, fileName);
            var fileLines = new List<string> { $"From: {from}" };

            foreach (var (to, message) in toMessages)
            {
                fileLines.Add($"To: {to}");
                fileLines.Add($"Message: {message}");
            }

            await fileManager.WriteFileAsync(filePath, string.Join("\n", fileLines));
        }
    }
}
