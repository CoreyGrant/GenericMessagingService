using GenericMessagingService.Services.Email.Services;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Template;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailRequest request);
    }

    internal class EmailSenderService : IEmailSenderService
    {
        private readonly ITemplateService templateService;
        private readonly EmailSettings emailSettings;
        private readonly IEmailService emailService;

        public EmailSenderService(
            EmailSettings emailSettings,
            ITemplateService templateService,
            IEmailStrategyResolver emailServiceResolver)
        {
            this.templateService = templateService;
            this.emailSettings = emailSettings;
            this.emailService = emailServiceResolver.Resolve();
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            var template = request.Template;
            var subject = request.Subject;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                // request contains a template name, find template then populate with object
                var tr = await this.templateService.GetTemplate(new TemplateRequest { Data = request.Data, TemplateName = request.TemplateName });
                template = tr.Body;
                if (!string.IsNullOrEmpty(tr.Subject))
                {
                    subject = tr.Subject;
                }
            }

            // send the email
            var toAddresses = !string.IsNullOrEmpty(emailSettings.OverrideToAddress)
                ? new List<string> { emailSettings.OverrideToAddress }
                : request.To;
            var fromAddress = !string.IsNullOrEmpty(request.From)
                ? request.From
                : emailSettings.DefaultFromAddress;
            await emailService.SendEmailAsync(
                subject,
                template,
                toAddresses,
                fromAddress);
        }
    }
}
