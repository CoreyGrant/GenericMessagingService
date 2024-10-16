using GenericMessagingService.Services.Email;
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
        public FolderSmsService(FolderSettings settings)
        {
            this.settings = settings;
        }

        public async Task SendSms(string message, IEnumerable<string> to, string from)
        {
            var fileName = Guid.NewGuid().ToString() + ".txt";
            var fileText = @$"From: {from}
To: {string.Join(", ", to)}
Message: {message}";
            await File.WriteAllTextAsync(fileName, fileText);
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

            await File.WriteAllLinesAsync(fileName, fileLines);
        }
    }
}
