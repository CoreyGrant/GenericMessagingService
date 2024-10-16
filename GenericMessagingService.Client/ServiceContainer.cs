using GenericMessagingService.Client.Utils;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<IEmailClient, EmailClient>();
            services.AddTransient<ISmsClient, SmsClient>();
            services.AddTransient<ITemplateClient, TemplateClient>();
            var converter = new ClassToDictionaryConverter();
            if(configureConverter != null)
            {
                configureConverter(converter);
            }
            services.AddSingleton<IClassToDictionaryConverter>(converter);
            return services;
        }
    }
}