using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;

namespace GenericMessagingService.Client
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddMessagingClient(
            this IServiceCollection services,
            ClientSettings settings)
        {
            services.AddSingleton(settings);
            services.AddTransient<IEmailClient, EmailClient>();
            services.AddTransient<ISmsClient, SmsClient>();
            services.AddTransient<ITemplateClient, TemplateClient>();
            return services;
        }
    }
}