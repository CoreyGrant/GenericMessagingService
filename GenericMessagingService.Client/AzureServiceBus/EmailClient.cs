using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Types.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.AzureServiceBus
{
    public class EmailClient : BaseClient, IEmailClient
    {
        public EmailClient(IServiceBusClient serviceBusClient, IClassToDictionaryConverter converter): base(serviceBusClient, converter) { }

        public async Task<bool> SendPlainEmail(
            string to,
            string body,
            string subject,
            string? from = null)
        {
            return await AddEmailMessage(new EmailRequest
            {
                To = new[] { to },
                Template = body,
                Subject = subject,
                From = from,
            });
        }

        public async Task<bool> SendPlainEmail(
            IList<string> to,
            string body,
            string subject,
            string? from = null)
        {
            return await AddEmailMessage(new EmailRequest
            {
                To = to,
                Template = body,
                Subject = subject,
                From = from
            });
        }

        public async Task<bool> SendTemplateEmail(
            string to,
            string templateName,
            IDictionary<string, string> data,
            string? subject = null,
            string? from = null)
        {
            return await AddEmailMessage(new EmailRequest
            {
                To = new[] { to },
                TemplateName = templateName,
                Subject = subject,
                Data = data,
                From = from,
            });
        }

        public async Task<bool> SendTemplateEmail<T>(
            string to,
            string templateName,
            T data,
            string? subject,
            string? from) where T : class
        {
            return await SendTemplateEmail(to, templateName, converter.Convert(data), subject, from);
        }

        public async Task<bool> SendTemplateEmail(
            IList<string> to,
            string templateName,
            IDictionary<string, string> data,
            string? subject = null,
            string? from = null)
        {
            return await AddEmailMessage(new EmailRequest
            {
                To = to,
                TemplateName = templateName,
                Data = data,
                Subject = subject,
                From = from
            });
        }

        public async Task<bool> SendTemplateEmail<T>(
            IList<string> to,
            string templateName,
            T data,
            string? subject = null,
            string? from = null) where T : class
        {
            return await SendTemplateEmail(to, templateName, converter.Convert(data), subject, from);
        }

        public async Task<bool> SendTemplateEmail(
            IList<string> to,
            string templateName,
            IDictionary<string, IDictionary<string, string>> toData,
            string? subject = null,
            string? from = null)
        {
            return await AddEmailMessage(new EmailRequest
            {
                To = to,
                TemplateName = templateName,
                ToData = toData,
                Subject = subject,
                From = from
            });
        }

        public async Task<bool> SendTemplateEmail<T>(
            IList<string> to,
            string templateName,
            IDictionary<string, T> data,
            string? subject = null,
            string? from = null) where T : class
        {
            return await SendTemplateEmail(to, templateName, data.ToDictionary(x => x.Key, x => converter.Convert(x.Value)), subject, from);
        }

        private async Task<bool> AddEmailMessage(EmailRequest emailRequest)
        {
            await this.serviceBusClient.AddEmailMessage(emailRequest);
            return true;
        }
    }
}
