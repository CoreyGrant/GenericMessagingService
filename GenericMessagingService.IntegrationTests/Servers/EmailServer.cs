using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Servers
{
    [Server("Email")]
    internal class EmailServer : BaseServer
    {
        public EmailServer(string hostUrl, string apiKey) : base(hostUrl, apiKey)
        {
        }

        protected override AppSettings AppSettings => new AppSettings
        {
            Email = new EmailSettings
            {
                Folder = new FolderSettings { FolderPath = "C:\\GMSTests\\Email"}
            },
            Template = new TemplateSettings
            {
                Location = new TemplateLocationSettings
                {
                    Folder = new FolderTemplateLocationSettings{
                        BaseFolder = Path.Join(AppContext.BaseDirectory, "\\Data\\Templates\\"),
                        Regex = new Dictionary<string, string>
                        {
                            ["^([A-Za-z0-9]+)$"] = "{0}.cshtml"
                        }
                    }
                },
                Formatting = new TemplateFormattingSettings
                {
                    Razor = new RazorTemplateFormattingSettings { }
                }
            }
        };
    }
}
