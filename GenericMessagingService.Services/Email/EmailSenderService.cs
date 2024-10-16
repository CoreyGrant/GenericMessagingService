using GenericMessagingService.Services.Email.Services;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Template;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(EmailRequest request);
    }

    internal class EmailSenderService : IEmailSenderService
    {
        private readonly ITemplateRunnerService templateRunnerService;
        private readonly EmailSettings emailSettings;
        private readonly IEmailService emailService;

        public EmailSenderService(
            EmailSettings emailSettings,
            ITemplateRunnerService templateRunnerService,
            IEmailStrategyResolver emailServiceResolver)
        {
            this.templateRunnerService = templateRunnerService;
            this.emailSettings = emailSettings;
            this.emailService = emailServiceResolver.Resolve();
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                if (request.Data != null) 
                {
                    var tr = await this.templateRunnerService.RunTemplate(request.TemplateName, request.Data);
                    if (!tr.HasValue)
                    {
                        throw new System.Exception("Failed to resolve template with name " + request.TemplateName);
                    }
                    var template = tr.Value.Body;
                    var subject = request.Subject;
                    if (!string.IsNullOrEmpty(tr.Value.Subject))
                    {
                        subject = tr.Value.Subject;
                    }
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
                } else if(request.ToData != null)
                {
                    var toSubjectBodys = new Dictionary<string, (string, string)>();
                    foreach (var (to, data) in request.ToData)
                    {
                        var tr = await this.templateRunnerService.RunTemplate(request.TemplateName, data);
                        if (!tr.HasValue)
                        {
                            throw new System.Exception("Failed to resolve template with name " + request.TemplateName);
                        }
                        var template = tr.Value.Body;
                        var subject = request.Subject;
                        if (!string.IsNullOrEmpty(tr.Value.Subject))
                        {
                            subject = tr.Value.Subject;
                        }
                        var toAddress = !string.IsNullOrEmpty(emailSettings.OverrideToAddress)
                           ? emailSettings.OverrideToAddress 
                           : to;
                        toSubjectBodys[to] = (subject, template);
                    }
                    var fromAddress = !string.IsNullOrEmpty(request.From)
                        ? request.From
                        : emailSettings.DefaultFromAddress;
                    await emailService.SendEmailAsync(toSubjectBodys, fromAddress);
                }
                throw new System.Exception("Was given a template name but no data");
            }
            else
            {
                var template = request.Template;
                var subject = request.Subject;
                var toAddresses = !string.IsNullOrEmpty(emailSettings.OverrideToAddress)
                        ? new List<string> { emailSettings.OverrideToAddress }
                        : request.To;
                var fromAddress = !string.IsNullOrEmpty(request.From)
                    ? request.From
                    : emailSettings.DefaultFromAddress;
                await emailService.SendEmailAsync(subject, template, toAddresses, fromAddress);
            }
        }
    }
}
