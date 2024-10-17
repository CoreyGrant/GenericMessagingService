using Azure.Messaging.ServiceBus;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Sms;
using GenericMessagingService.Types.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AzureServiceBusClient = Azure.Messaging.ServiceBus.ServiceBusClient;

namespace GenericMessagingService.Client.AzureServiceBus
{
    public interface IServiceBusClient
    {
        Task AddEmailMessage(EmailRequest message);
        Task AddSmsMessage(SmsRequest message);
        Task<TemplateResponse?> AddTemplateMessage(TemplateRequest message);
    }

    internal class ServiceBusClient : IServiceBusClient
    {
        private readonly AzureServiceBusClient client;
        private readonly ServiceBusSender emailSender;
        private readonly ServiceBusSender smsSender;
        private readonly ServiceBusSender templateSender;
        private readonly ServiceBusReceiver templateReceiver;

        public ServiceBusClient(AzureServiceBusClientSettings settings)
        {
            client = new AzureServiceBusClient(
                settings.ConnectionString);
            
            emailSender = client.CreateSender(settings.EmailQueueName);
            smsSender = client.CreateSender(settings.SmsQueueName);
            templateSender = client.CreateSender(settings.TemplateQueueName);
            templateReceiver = client.CreateReceiver(settings.TemplateResponseQueueName);
        }

        public async Task AddEmailMessage(EmailRequest message)
        {
            var messageJson = JsonConvert.SerializeObject(message);
            
            await emailSender.SendMessageAsync(new ServiceBusMessage
            {
                Body = BinaryData.FromString(messageJson)
            });
        }

        public async Task AddSmsMessage(SmsRequest message)
        {
            var messageJson = JsonConvert.SerializeObject(message);

            await smsSender.SendMessageAsync(new ServiceBusMessage
            {
                Body = BinaryData.FromString(messageJson)
            });
        }

        public async Task<TemplateResponse?> AddTemplateMessage(TemplateRequest message)
        {
            var messageJson = JsonConvert.SerializeObject(message);
            var messageGuid = Guid.NewGuid().ToString();
            await templateSender.SendMessageAsync(new ServiceBusMessage
            {
                Subject = messageGuid,
                Body = BinaryData.FromString(messageJson)
            });
            var messages = templateReceiver.ReceiveMessagesAsync();
            await foreach(var messageResponse in messages)
            {
                if(messageResponse.Subject == messageGuid)
                {
                    try
                    {
                        await templateReceiver.CompleteMessageAsync(messageResponse);
                        return JsonConvert.DeserializeObject<TemplateResponse>(messageResponse.ToString());
                    } catch(Exception ex)
                    {
                        await templateReceiver.DeadLetterMessageAsync(messageResponse);
                    }
                }
            }
            return null;
        }

    }
}
