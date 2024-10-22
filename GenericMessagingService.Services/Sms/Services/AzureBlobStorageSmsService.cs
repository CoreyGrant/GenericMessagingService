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
    internal class AzureBlobStorageSmsService : ISmsService
    {
        private readonly AzureBlobStorageSettings settings;
        private readonly IAzureBlobStorageManager azureBlobStorageManager;

        public AzureBlobStorageSmsService(
            AzureBlobStorageSettings settings,
            IAzureBlobStorageManager azureBlobStorageManager)
        {
            this.settings = settings;
            this.azureBlobStorageManager = azureBlobStorageManager;
        }

        public async Task SendSms(string message, IEnumerable<string> to, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            var fileText = @$"From: {from}
To: {string.Join(", ", to)}
Message: {message}";
            await azureBlobStorageManager.StoreFile(fileText, settings.FolderPath, fileName);
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
            await azureBlobStorageManager.StoreFile(string.Join("\n", fileLines), settings.FolderPath, fileName);
        }
    }
}
