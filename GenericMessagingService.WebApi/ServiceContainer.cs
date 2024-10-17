using GenericMessagingService.Services;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi.Controllers;
using GenericMessagingService.WebApi.Setup;
using Newtonsoft.Json;

namespace GenericMessagingService.WebApi
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services, GenericMessagingServiceSettings settings)
        {
            AppSettings? config = null;
            if(settings.Config != null)
            {
                config = settings.Config;
            } else if (!string.IsNullOrEmpty(settings.ConfigPath))
            {
                var path = Path.Combine(AppContext.BaseDirectory, settings.ConfigPath);
                config = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path))!;
            }
            if(config == null)
            {
                throw new Exception();
            }
            services.AddAppSettings(config);
            services.AddMessagingServices();
            if (settings.BindingType.HasFlag(BindingType.Web))
            {
                if (config.Email != null)
                {
                    services.AddTransient<EmailController>();
                }
                if(config.Sms != null)
                {
                    services.AddTransient<SmsController>();
                }
                if(config.Template != null || config.ComboTemplate != null)
                {
                    services.AddTransient<TemplateController>();
                }
            }
            if (settings.BindingType.HasFlag(BindingType.AzureServiceBus))
            {

            }
            
            return services;
        }

        private static void AddAppSettings(this IServiceCollection services, AppSettings appSettings)
        {
            var logger = new Logger<ConfigValidator>(new LoggerFactory());
            var validator = new ConfigValidator(logger);
            validator.Validate(appSettings);

            services.AddSingleton(appSettings);
            services.AddSingleton(appSettings.Email);
            services.AddSingleton(appSettings.Sms);
            services.AddSingleton(appSettings.Template);
            services.AddSingleton(appSettings.ComboTemplate);
        }
    }
}
