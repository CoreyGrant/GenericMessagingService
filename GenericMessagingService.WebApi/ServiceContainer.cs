using GenericMessagingService.Services;
using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Sms;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi.AzureServiceBus;
using GenericMessagingService.WebApi.Controllers;
using GenericMessagingService.WebApi.Filters;
using GenericMessagingService.WebApi.Setup;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;

namespace GenericMessagingService.WebApi
{
    public static class ServiceContainer
    {
        public static WebApplication UseWebApi(this WebApplication webApplication, GenericMessagingServiceSettings settings)
        {
            if (settings.BindingType.HasFlag(BindingType.Web)) 
            {
                webApplication.UseRouting();
                webApplication.UseEndpoints(o => o.MapControllers());
            }
            if (settings.BindingType.HasFlag(BindingType.AzureServiceBus))
            {
                var sbc = new ServiceBusClient(
                    settings.AzureServiceBus,
                    webApplication.Services.GetService<IEmailSenderService>()!,
                    webApplication.Services.GetService<ISmsSenderService>()!,
                    webApplication.Services.GetService<ITemplateRunnerService>()!);
                sbc.StartClient();
            }
            return webApplication;
        }

        public static IServiceCollection AddWebApi(this IServiceCollection services, GenericMessagingServiceSettings settings)
        {
            services.AddSingleton(settings);
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
                if (string.IsNullOrEmpty(settings.ApiKey))
                {
                    throw new ConfigValidationException("ApiKey is required for Web");
                }
                
                services.AddControllers();
            }
            if (settings.BindingType.HasFlag(BindingType.AzureServiceBus))
            {
                if(settings.AzureServiceBus == null)
                {
                    throw new ConfigValidationException("AzureServiceBus config is required");
                }
                services.AddSingleton(settings.AzureServiceBus);
                services.AddSingleton<IServiceBusClient, ServiceBusClient>();
            }

            return services;
        }

        private static void AddAppSettings(this IServiceCollection services, AppSettings appSettings)
        {
            var logger = new Logger<ConfigValidator>(new LoggerFactory());
            var validator = new ConfigValidator(logger);
            validator.Validate(appSettings);

            services.AddSingleton(appSettings);
            if (appSettings.Email != null)
            {
                services.AddSingleton(appSettings.Email);
            }
            if (appSettings.Sms != null)
            {
                services.AddSingleton(appSettings.Sms);
            }
            if (appSettings.Template != null)
            {
                services.AddSingleton(appSettings.Template);
            }
            if (appSettings.ComboTemplate != null)
            {
                services.AddSingleton(appSettings.ComboTemplate);
            }
        }

        //private static void UseController(WebApplication webApplication, Type controller, string apiKey, string route = null)
        //{
        //    if(route == null)
        //    {
        //        var routeAttribute = controller.GetCustomAttribute<RouteAttribute>()!;
        //        route = routeAttribute.Template.Replace("[controller]", controller.Name.Replace("Controller", ""));
        //    }
        //    var controllerMethods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //    foreach(var method in controllerMethods)
        //    {
        //        var postRoute = method.GetCustomAttribute<HttpPostAttribute>();
        //        if(postRoute == null) { continue; }
        //        var paramType = method.GetParameters().First().ParameterType;
        //        webApplication.MapPost(route, async (HttpRequest request, HttpResponse response) =>
        //        {
        //            var xApiKey = request.Headers.ContainsKey("X-API-KEY") ? request.Headers["X-API-KEY"].ToString() : null;
        //            if (xApiKey == null)
        //            {
        //                response.StatusCode = (int)HttpStatusCode.BadRequest;
        //                return;
        //            }
        //            if (xApiKey != apiKey)
        //            {
        //                response.StatusCode = (int)HttpStatusCode.Forbidden;
        //                return;
        //            }
        //            var controllerInstance = webApplication.Services.GetService(controller);
        //            response.StatusCode = 200;

        //            using (var tr = new StreamReader(request.Body)) 
        //            {
        //                var jsonStr = await tr.ReadToEndAsync();
        //                var jsonObj = JsonConvert.DeserializeObject(jsonStr, paramType);
        //                if (method.ReturnType == typeof(Task) || method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        //                {
        //                    var result = await ((Task<object>)method.Invoke(controllerInstance, new[] { jsonObj }));
        //                    await response.WriteAsJsonAsync(result);
        //                } else
        //                {
        //                    var result = method.Invoke(controllerInstance, new[] { jsonObj });
        //                    await response.WriteAsJsonAsync(result);
        //                }
        //            }
        //        });
        //    }
        //}
    }
}
