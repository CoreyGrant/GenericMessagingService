using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Templating;
using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using Microsoft.Extensions.DependencyInjection;
using RazorEngineCore;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection container)
        {
            container.AddTransient<ITemplateServiceFactory, TemplateServiceFactory>();
            container.AddTransient<IComboTemplateServiceFactory, ComboTemplateServiceFactory>();
            container.AddTransient<ITemplateRunnerService, TemplateRunnerService>();
            container.AddTransient<IFileManager, FileManager>();
            container.AddTransient<IEmailSenderService, EmailSenderService>();
            container.AddSingleton<IEmailStrategyResolver, EmailStrategyResolver>();
            container.AddSingleton<ITemplateLocationServiceResolver, TemplateLocationServiceResolver>();
            container.AddSingleton<ITemplateFormattingServiceResolver, TemplateFormattingServiceResolver>();
            container.AddSingleton<IDatabaseStrategyResolver, DatabaseStrategyResolver>();
            container.AddSingleton<ISendGridClient>((a) => new SendGridClient(new SendGridClientOptions
            {
                ApiKey = a.GetService<EmailSettings>().SendGrid.ApiKey
            }));
            container.AddTransient<IRazorEngine, RazorEngine>();
            return container;
        }
    }
}
