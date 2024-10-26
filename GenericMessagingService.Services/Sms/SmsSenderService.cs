using GenericMessagingService.Services.Sms.Services;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Sms;
using GenericMessagingService.Types.Template;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Sms
{
    public interface ISmsSenderService
    {
        Task SendSmsAsync(SmsRequest request);
    }

    [InjectScoped(ServiceType.Sms)]
    internal class SmsSenderService : ISmsSenderService
    {
        private readonly SmsSettings settings;
        private readonly ITemplateRunnerService templateRunnerService;
        private readonly ISmsService smsService;

        public SmsSenderService(
            AppSettings settings,
            ITemplateRunnerService templateRunnerService,
            ISmsStrategyResolver smsStrategyResolver,
            ITemplateStrategyService templateStrategyService)
        {
            templateStrategyService.TemplateStrategy = settings.Sms.TemplateStrategy;
            this.settings = settings.Sms;
            this.templateRunnerService = templateRunnerService;
            this.smsService = smsStrategyResolver.Resolve();
        }

        public async Task SendSmsAsync(SmsRequest request)
        {
            // has template
            // has template name with data
            // has template name with toData
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                if (request.Data != null)
                {
                    // request contains a template name, find template then populate with object
                    var tr = await this.templateRunnerService.RunTemplate(request.TemplateName, request.Data);
                    if (!tr.HasValue)
                    {
                        throw new System.Exception("Failed to resolve template with name " + request.TemplateName);
                    }
                    var template = tr.Value.Body;
                    var toAddresses = !string.IsNullOrEmpty(settings.OverrideToNumber)
                    ? new List<string> { settings.OverrideToNumber }
                    : request.To;
                    var fromAddress = !string.IsNullOrEmpty(request.From)
                        ? request.From
                        : settings.DefaultFromNumber;
                    await smsService.SendSms(template, toAddresses, fromAddress);
                }
                else if (request.ToData != null)
                {
                    var toMessages = new Dictionary<string, string>();
                    foreach (var (to, data) in request.ToData)
                    {
                        var tr = await this.templateRunnerService.RunTemplate(request.TemplateName, data);
                        if (!tr.HasValue)
                        {
                            throw new System.Exception("Failed to resolve template with name " + request.TemplateName);
                        }
                        var template = tr.Value.Body;
                        var toAddress = !string.IsNullOrEmpty(settings.OverrideToNumber)
                            ? settings.OverrideToNumber
                            : to;
                        toMessages[toAddress] = template;
                    }
                    var fromAddress = !string.IsNullOrEmpty(request.From)
                            ? request.From
                            : settings.DefaultFromNumber;
                    await smsService.SendSms(toMessages, fromAddress);
                }
                else
                {
                    throw new System.Exception("Was given a template name but no data");
                }
            } else
            {
                var toAddresses = !string.IsNullOrEmpty(settings.OverrideToNumber)
                    ? new List<string> { settings.OverrideToNumber }
                    : request.To;
                var fromAddress = !string.IsNullOrEmpty(request.From)
                    ? request.From
                    : settings.DefaultFromNumber;
                await smsService.SendSms(request.Template, toAddresses, fromAddress);
            }
        }
    }
}
