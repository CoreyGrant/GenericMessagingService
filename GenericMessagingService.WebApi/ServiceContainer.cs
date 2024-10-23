using GenericMessagingService.Services;
using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Pdf;
using GenericMessagingService.Services.Sms;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Pdf;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Sms;
using GenericMessagingService.Types.Template;
using GenericMessagingService.WebApi.AzureServiceBus;
using GenericMessagingService.WebApi.Filters;
using GenericMessagingService.WebApi.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                var config = settings.Config;
                if (config.Template != null || config.ComboTemplate != null)
                {
                    RegisterTemplate(webApplication, settings.ApiKey);
                }
                if(config.Sms != null)
                {
                    RegisterSms(webApplication, settings.ApiKey);
                }
                if(config.Pdf != null)
                {
                    RegisterPdf(webApplication, settings.ApiKey);
                }
                if (config.Email != null) 
                {
                    RegisterEmail(webApplication, settings.ApiKey);
                }
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
                settings.Config = config;
            }
            if(config == null)
            {
                throw new Exception();
            }
            services.AddAppSettings(config);
            services.AddMessagingServices(config);
            if (settings.BindingType.HasFlag(BindingType.Web))
            {
                if (string.IsNullOrEmpty(settings.ApiKey))
                {
                    throw new ConfigValidationException("ApiKey is required for Web");
                }
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
            if(appSettings.Pdf != null)
            {
                services.AddSingleton(appSettings.Pdf);
            }
        }

        private static void RegisterEmail(WebApplication app, string apiKey)
        {
            app.Services.GetService<IEmailSenderService>();
            app.MapPost("/api/email", async (HttpRequest req, HttpResponse res) =>
            {
                var guard = ApiGuard(req, apiKey);
                if (guard != null) { res.StatusCode = (int)guard; return; }
                using (var sr = new StreamReader(req.Body)) 
                {
                    var bodyText = await sr.ReadToEndAsync();
                    var emailRequest = JsonConvert.DeserializeObject<EmailRequest>(bodyText);
                    var emailSenderService = app.Services.GetService<IEmailSenderService>()!;
                    ApiResponse result;
                    try
                    {
                        await emailSenderService.SendEmailAsync(emailRequest);
                        result = new ApiResponse(true);
                    } catch(Exception ex)
                    {
                        result = new ApiResponse(ex.Message);
                        res.StatusCode = 500;
                    }
                    await res.WriteAsJsonAsync(result);
                }
            });
        }

        private static void RegisterSms(WebApplication app, string apiKey)
        {
            app.Services.GetService<ISmsSenderService>();
            app.MapPost("/api/sms", async (HttpRequest req, HttpResponse res) =>
            {
                var guard = ApiGuard(req, apiKey);
                if (guard != null) { res.StatusCode = (int)guard; return; }
                using (var sr = new StreamReader(req.Body))
                {
                    var bodyText = await sr.ReadToEndAsync();
                    var smsRequest = JsonConvert.DeserializeObject<SmsRequest>(bodyText);
                    var smsSenderService = app.Services.GetService<ISmsSenderService>()!;
                    ApiResponse result;
                    try
                    {
                        await smsSenderService.SendSmsAsync(smsRequest);
                        result = new ApiResponse(true);
                    }
                    catch (Exception ex)
                    {
                        result = new ApiResponse(ex.Message);
                        res.StatusCode = 500;
                    }
                    await res.WriteAsJsonAsync(result);
                }
            });
        }

        private static void RegisterTemplate(WebApplication app, string apiKey)
        {
            app.Services.GetService<ITemplateRunnerService>();
            app.MapPost("/api/template", async (HttpRequest req, HttpResponse res) =>
            {
                var guard = ApiGuard(req, apiKey);
                if (guard != null) { res.StatusCode = (int)guard; return; }
                using (var sr = new StreamReader(req.Body))
                {
                    var bodyText = await sr.ReadToEndAsync();
                    var templateRequest = JsonConvert.DeserializeObject<TemplateRequest>(bodyText);
                    var templateRunnerService = app.Services.GetService<ITemplateRunnerService>()!;
                    ApiResponse<TemplateResponse> result;
                    try
                    {
                        var templateResult = await templateRunnerService.RunTemplate(templateRequest.TemplateName, templateRequest.Data);
                        if (templateResult == null)
                        {
                            result = new ApiResponse<TemplateResponse>("Failed to find template");
                        }
                        else
                        {
                            result = new ApiResponse<TemplateResponse>(new TemplateResponse { Body = templateResult.Value.Body, Subject = templateResult.Value.Subject });
                        }
                    }
                    catch (Exception ex)
                    {
                        res.StatusCode = 500;
                        result = new ApiResponse<TemplateResponse>(ex.Message);
                    }
                    await res.WriteAsJsonAsync(result);
                }
            });
            app.MapGet("/api/names", async (HttpRequest req, HttpResponse res) =>
            {
                var templateRunnerService = app.Services.GetService<ITemplateRunnerService>()!;
                ApiResponse<List<string>> result;
                try
                {
                    var templateNameResult = await templateRunnerService.GetTemplateNames();
                    result = new ApiResponse<List<string>>(templateNameResult);
                } catch(Exception ex)
                {
                    result = new ApiResponse<List<string>>(ex.Message);
                    res.StatusCode = 500;
                }
                await res.WriteAsJsonAsync(result);
            });
        }

        private static void RegisterPdf(WebApplication app, string apiKey)
        {
            app.Services.GetService<ISmsSenderService>();
            app.MapPost("/api/pdf", async (HttpRequest req, HttpResponse res) =>
            {
                var guard = ApiGuard(req, apiKey);
                if (guard != null) { res.StatusCode = (int)guard; return; }
                using (var sr = new StreamReader(req.Body))
                {
                    var bodyText = await sr.ReadToEndAsync();
                    var pdfRequest = JsonConvert.DeserializeObject<PdfRequest>(bodyText);
                    var pdfGenerationService = app.Services.GetService<IPdfGenerationService>()!;
                    ApiResponse result;
                    try
                    {
                        var filename = string.IsNullOrEmpty(pdfRequest.Filename)
                            ? pdfRequest.TemplateName.Replace("\\", "_").Replace("/", "_") + ".pdf"
                            : pdfRequest.Filename;
                        var pdfBytes = await pdfGenerationService.GetPdf(pdfRequest.TemplateName, pdfRequest.Data, filename);

                        res.Headers.ContentType = "application/pdf";
                        res.Headers.ContentDisposition = filename;
                        await res.BodyWriter.WriteAsync(pdfBytes);
                    }
                    catch (Exception ex)
                    {
                        res.StatusCode = 500;
                    }
                }
            });
        }

        private const string XApiKey = "X-API-KEY";
        private static HttpStatusCode? ApiGuard(HttpRequest request, string apiKey)
        {
            var xApiKey = request.Headers.ContainsKey("X-API-KEY") ? request.Headers["X-API-KEY"].ToString() : null;
            if (xApiKey == null)
            {
                return HttpStatusCode.BadRequest;
                
            }
            if (xApiKey != apiKey)
            {
                return HttpStatusCode.Forbidden;
            }
            return null;
        }
    }
}
