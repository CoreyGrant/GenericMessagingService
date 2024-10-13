using GenericMessagingService.Services.Sms.Services;
using GenericMessagingService.Services.Templating.Services;
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

    internal class SmsSenderService : ISmsSenderService
    {
        private readonly SmsSettings settings;
        private readonly ITemplateService templateService;
        private readonly ISmsService smsService;

        public SmsSenderService(
            AppSettings settings,
            ITemplateService templateService,
            ISmsStrategyResolver smsStrategyResolver)
        {
            this.settings = settings.Sms;
            this.templateService = templateService;
            this.smsService = smsStrategyResolver.Resolve();
        }

        public async Task SendSmsAsync(SmsRequest request)
        {
            var template = request.Template;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                // request contains a template name, find template then populate with object
                var tr = await this.templateService.GetTemplate(new TemplateRequest { Data = request.Data, TemplateName = request.TemplateName });
                template = tr.Body;
            }
            var toAddresses = !string.IsNullOrEmpty(settings.OverrideToNumber)
                ? new List<string> { settings.OverrideToNumber }
                : request.To;
            var fromAddress = !string.IsNullOrEmpty(request.From)
                ? request.From
                : settings.DefaultFromNumber;
            await smsService.SendSms(template, toAddresses, fromAddress);
        }
    }
}
