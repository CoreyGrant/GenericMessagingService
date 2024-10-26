using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Servers
{
    [Server("Full")]
    internal class FullServer : BaseServer
    {
        protected override AppSettings AppSettings => new AppSettings
        {
            Email = new EmailSettings
            {
                TemplateStrategy = "NewEmail|OldEmail",
                Folder = new FolderSettings { FolderPath = "C:\\GMSTests\\Email" }
            },
            Pdf = new PdfSettings
            {
                TemplateStrategy = "Pdf",
                Folder = new FolderSettings { FolderPath = "C:\\GMSTests\\Pdf" },
                ChromeExePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            },
            Sms = new SmsSettings
            {
                TemplateStrategy = "Sms",
                Folder = new FolderSettings { FolderPath = "C:\\GMSTests\\Sms" }
            },
            ComboTemplate = new ComboTemplateSettings
            {
                Strategy = "OldEmail|NewEmail|Pdf|Sms",
                Combo = new Dictionary<string, TemplateSettings>
                {
                    ["OldEmail"] = new TemplateSettings
                    {
                        Location = new TemplateLocationSettings
                        {
                            Folder = new FolderTemplateLocationSettings
                            {
                                FolderPath = Path.Join(AppContext.BaseDirectory, "\\Data\\Templates\\Email\\OldEmail"),
                                NameAsPath = true,
                            }
                        },
                        Formatting = new TemplateFormattingSettings { Basic = new BasicTemplateFormattingSettings()}
                    },
                    ["NewEmail"] = new TemplateSettings
                    {
                        Location = new TemplateLocationSettings
                        {
                            Folder = new FolderTemplateLocationSettings
                            {
                                FolderPath = Path.Join(AppContext.BaseDirectory, "\\Data\\Templates\\Email"),
                                NameAsPath = true,
                            }
                        },
                        Formatting = new TemplateFormattingSettings { Razor = new RazorTemplateFormattingSettings()}
                    },
                    ["Pdf"] = new TemplateSettings
                    {
                        Location = new TemplateLocationSettings
                        {
                            Folder = new FolderTemplateLocationSettings
                            {
                                FolderPath = Path.Join(AppContext.BaseDirectory, "\\Data\\Templates\\Pdf"),
                                NameAsPath = true,
                            }
                        },
                        Formatting = new TemplateFormattingSettings { Razor = new RazorTemplateFormattingSettings() }
                    },
                    ["Sms"] = new TemplateSettings
                    {
                        Location = new TemplateLocationSettings
                        {
                            Folder = new FolderTemplateLocationSettings
                            {
                                FolderPath = Path.Join(AppContext.BaseDirectory, "\\Data\\Templates\\Sms"),
                                NameAsPath = true,
                            }
                        },
                        Formatting = new TemplateFormattingSettings { Basic = new BasicTemplateFormattingSettings() }
                    }
                }
            }
        };
    }
}
