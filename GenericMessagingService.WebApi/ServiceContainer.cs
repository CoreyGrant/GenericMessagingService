using GenericMessagingService.Services;
using GenericMessagingService.Types.Config;
using Newtonsoft.Json;

namespace GenericMessagingService.WebApi
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            var configFilePath = Path.Combine(AppContext.BaseDirectory, "defaultConfig.json");
            var appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(configFilePath))!;
            services.AddSingleton(appSettings);
            services.AddSingleton(appSettings.Email);
            services.AddSingleton(appSettings.Sms);
            services.AddSingleton(appSettings.Template);
            services.AddSingleton(appSettings.ComboTemplate);
            services.AddMessagingServices();
            return services;
        }
    }
}
