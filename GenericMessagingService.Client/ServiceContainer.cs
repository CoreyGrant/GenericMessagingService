using GenericMessagingService.Client.AzureServiceBus;
using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Client.Web;
using Microsoft.Extensions.DependencyInjection;
using AsbEmailClient = GenericMessagingService.Client.AzureServiceBus.EmailClient;
using AsbSmsClient = GenericMessagingService.Client.AzureServiceBus.SmsClient;
using AsbTemplateClient = GenericMessagingService.Client.AzureServiceBus.TemplateClient;
using HttpEmailClient = GenericMessagingService.Client.Web.EmailClient;
using HttpSmsClient = GenericMessagingService.Client.Web.SmsClient;
using HttpTemplateClient = GenericMessagingService.Client.Web.TemplateClient;
using System.ComponentModel.Design;

namespace GenericMessagingService.Client
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddMessagingClient(
            this IServiceCollection services,
            ClientSettings settings,
            Action<IClassToDictionaryConverter> configureConverter = null)
        {
            services.AddSingleton(settings);
            var converter = new ClassToDictionaryConverter();
            if (configureConverter != null)
            {
                configureConverter(converter);
            }
            services.AddSingleton<IClassToDictionaryConverter>(converter);

            if (settings.Web != null)
            {
                services.AddTransient<IEmailClient, HttpEmailClient>();
                services.AddTransient<ISmsClient, HttpSmsClient>();
                services.AddTransient<ITemplateClient, HttpTemplateClient>();
            } else if(settings.AzureServiceBus != null){
                services.AddTransient<IServiceBusClient, ServiceBusClient>();
                services.AddTransient<IEmailClient, AsbEmailClient>();
                services.AddTransient<ISmsClient, AsbSmsClient>();
                services.AddTransient<ITemplateClient, AsbTemplateClient>();
            }
            
            return services;
        }
    }
}