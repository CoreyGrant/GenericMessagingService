using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Sms;
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
            container.AddTransient<IHashService, HashSerice>();
            container.AddTransient<IEmailSenderService, EmailSenderService>();
            container.AddTransient<IEmailStrategyResolver, EmailStrategyResolver>();
            container.AddTransient<ITemplateLocationServiceResolver, TemplateLocationServiceResolver>();
            container.AddTransient<ITemplateFormattingServiceResolver, TemplateFormattingServiceResolver>();
            container.AddTransient<IDatabaseStrategyResolver, DatabaseStrategyResolver>();
            container.AddTransient<ISmsSenderService, SmsSenderService>();
            container.AddTransient<ISmsStrategyResolver, SmsStrategyResolver>();
            container.AddSingleton<ISendGridClient>((a) => new SendGridClient(new SendGridClientOptions
            {
                ApiKey = a.GetService<EmailSettings>().SendGrid.ApiKey
            }));
            container.AddTransient<IRazorEngine, RazorEngine>();
            container.AddSingleton<ICacheManager, MemoryCacheManager>();
            return container;
        }
    }
}
