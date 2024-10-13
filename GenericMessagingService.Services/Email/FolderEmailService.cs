using GenericMessagingService.Types.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email
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
            var fileName = Guid.NewGuid().ToString() + ".json";
            var obj = new
            {
                subject,
                body,
                to,
                from
            };
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            await File.WriteAllTextAsync(Path.Combine(settings.FolderPath, fileName), json);
        }
    }
}
