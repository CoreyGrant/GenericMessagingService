using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.StorageService;
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
    internal class FileSmsService : ISmsService
    {
        private readonly FolderSettings settings;
        private readonly IStorageService storageService;

        public FileSmsService(FolderSettings settings, IStorageService storageService)
        {
            this.settings = settings;
            this.storageService = storageService;
        }

        public async Task SendSms(string message, IEnumerable<string> to, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            var fileText = @$"From: {from}
To: {string.Join(", ", to)}
Message: {message}";
            await storageService.StoreFile(fileText, settings.FolderPath, fileName);
        }

        public async Task SendSms(Dictionary<string, string> toMessages, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            var fileLines = new List<string> { $"From: {from}" };

            foreach (var (to, message) in toMessages)
            {
                fileLines.Add($"To: {to}");
                fileLines.Add($"Message: {message}");
            }

            await storageService.StoreFile(string.Join("\n", fileLines), settings.FolderPath, fileName);
        }
    }
}
