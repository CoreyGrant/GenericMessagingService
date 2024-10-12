using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Templating;
using GenericMessagingService.Services.Templating.Database;
using Microsoft.Extensions.DependencyInjection;
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
            container.AddTransient<ITemplateService, TemplateService>();
            container.AddTransient<IEmailSenderService, EmailSenderService>();
            container.AddTransient<IEmailStrategyResolver, EmailStrategyResolver>();
            container.AddTransient<ITemplateStrategyResolver, TemplateStrategyResolver>();
            container.AddTransient<IDatabaseStrategyResolver, DatabaseStrategyResolver>();
            return container;
        }
    }
}
