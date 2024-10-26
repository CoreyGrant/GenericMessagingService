using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Sms;
using GenericMessagingService.Services.Templating;
using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using Microsoft.Extensions.DependencyInjection;
using RazorEngineCore;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection container, AppSettings config)
        {
            var assembly = Assembly.GetAssembly(typeof(ServiceContainer));
            var neededServiceTypes = new List<ServiceType> { ServiceType.Default};
            if(config.Sms != null) { neededServiceTypes.Add(ServiceType.Sms); }
            if(config.Template != null || config.ComboTemplate != null)
            {
                neededServiceTypes.Add(ServiceType.Template);
            }
            if(config.Email != null) { neededServiceTypes.Add(ServiceType.Email); }
            if(config.Pdf != null) { neededServiceTypes.Add(ServiceType.Pdf);}
            var injectableTypes = assembly.GetTypes().Where(x => x.GetCustomAttribute<InjectAttribute>() != null);
            foreach(var injectType in injectableTypes)
            {
                var typeConcrete = injectType;
                var typeInterface = injectType.GetInterfaces().SingleOrDefault();
                if(typeInterface == null) { continue; }

                var transient = injectType.GetCustomAttribute<InjectTransientAttribute>();
                if(transient != null)
                {
                    if (neededServiceTypes.Any(x => transient.type == x))
                    {
                        container.AddTransient(typeInterface, typeConcrete);
                    }
                }
                var singleton = injectType.GetCustomAttribute<InjectSingletonAttribute>();
                if(singleton != null)
                {
                    if (neededServiceTypes.Any(x => singleton.type == x))
                    {
                        container.AddSingleton(typeInterface, typeConcrete);
                    }
                }

                var scoped = injectType.GetCustomAttribute<InjectScopedAttribute>();
                if(scoped != null)
                {
                    if(neededServiceTypes.Any(x => scoped.type == x))
                    {
                        container.AddScoped(typeInterface, typeConcrete);
                    }
                }
            }

            container.AddSingleton<ISendGridClient>((a) => new SendGridClient(new SendGridClientOptions
            {
                ApiKey = a.GetService<EmailSettings>()?.SendGrid?.ApiKey ?? "test"
            }));
            container.AddTransient<IRazorEngine, RazorEngine>();
            return container;
        }
    }
}
