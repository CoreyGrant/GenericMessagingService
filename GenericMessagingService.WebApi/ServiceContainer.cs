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
            services.AddSingleton(appSettings.Email?.SendGrid ?? new SendGridSettings());
            services.AddSingleton(appSettings.Email?.MailChimp ?? new MailChimpSettings());
            services.AddSingleton(appSettings.Email?.Smtp ?? new SMTPEmailSettings());
            services.AddSingleton(appSettings.Email?.Folder ?? new FolderSettings());
            services.AddSingleton(appSettings.Template?.DatabaseTemplates?? new DatabaseTemplateSettings());
            services.AddSingleton(appSettings.Template?.RazorTemplates ?? new RazorTemplateSettings());
            services.AddSingleton(appSettings.Sms?.Twilio ?? new TwilioSettings());
            return services;
        }
    }
}
