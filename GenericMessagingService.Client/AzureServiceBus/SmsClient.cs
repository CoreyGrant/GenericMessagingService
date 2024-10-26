using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.AzureServiceBus
{
    internal class SmsClient : BaseClient, ISmsClient
    {
        public SmsClient(IServiceBusClient serviceBusClient, IClassToDictionaryConverter converter) : base(serviceBusClient, converter)
        {
        }

        public async Task<bool> SendPlainSms(
            string to,
            string message,
            string? from = null)
        {
            return await AddSmsMessage(new SmsRequest
            {
                To = new[] { to },
                Template = message,
                From = from
            });
        }

        public async Task<bool> SendPlainSms(
            IList<string> to,
            string message,
            string? from = null)
        {
            return await AddSmsMessage(new SmsRequest
            {
                To = to,
                Template = message,
                From = from
            });
        }

        public async Task<bool> SendTemplateSms(
            string to,
            string templateName,
            IDictionary<string, string> data,
            string? from = null)
        {
            return await AddSmsMessage(new SmsRequest
            {
                To = new[] { to },
                TemplateName = templateName,
                From = from,
                Data = data,
            });
        }

        public async Task<bool> SendTemplateSms<T>(string to, string templateName, T data, string? from = null) where T : class
        {
            return await SendTemplateSms(to, templateName, converter.Convert(data), from);
        }

        public async Task<bool> SendTemplateSms(
            IList<string> to,
            string templateName,
            IDictionary<string, string> data,
            string? from = null)
        {
            return await AddSmsMessage(new SmsRequest
            {
                To = to,
                TemplateName = templateName,
                Data = data,
                From = from,
            });
        }

        public async Task<bool> SendTemplateSms<T>(
            IList<string> to,
            string templateName,
            T data,
            string? from = null) where T : class
        {
            return await SendTemplateSms(to, templateName, converter.Convert(data), from);
        }

        public async Task<bool> SendTemplateSms(
            IList<string> to,
            string templateName,
            IDictionary<string, IDictionary<string, string>> toData,
            string? from = null)
        {
            return await AddSmsMessage(new SmsRequest
            {
                To = to,
                TemplateName = templateName,
                ToData = toData,
                From = from
            });
        }

        public async Task<bool> SendTemplateSms<T>(
            IList<string> to,
            string templateName,
            IDictionary<string, T> data,
            string? from = null) where T : class
        {
            return await SendTemplateSms(to, templateName, data.ToDictionary(x => x.Key, x => converter.Convert(x.Value)), from);
        }

        private async Task<bool> AddSmsMessage(SmsRequest smsRequest)
        {
            await this.serviceBusClient.AddSmsMessage(smsRequest);
            return true;
        }
    }
}
