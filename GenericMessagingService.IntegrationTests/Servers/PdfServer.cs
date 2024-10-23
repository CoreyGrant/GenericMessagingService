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
    [Server("Pdf")]
    internal class PdfServer : BaseServer
    {
        public PdfServer(string hostUrl, string apiKey) : base(hostUrl, apiKey)
        {
        }

        protected override AppSettings AppSettings => new AppSettings
        {
            Pdf = new PdfSettings
            {
                Folder = new FolderSettings { FolderPath = "C:\\GMSTests\\Pdf"},
                ChromeExePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
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
