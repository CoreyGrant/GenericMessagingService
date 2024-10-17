using Azure.Messaging.ServiceBus;
using GenericMessagingService.Services.Email;
using GenericMessagingService.Services.Sms;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Sms;
using GenericMessagingService.Types.Template;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using AzureServiceBusClient = Azure.Messaging.ServiceBus.ServiceBusClient;


namespace GenericMessagingService.WebApi.AzureServiceBus
{
    public interface IServiceBusClient
    {
        Task StartClient();
    }

    public class ServiceBusClient : IServiceBusClient
    {
        private readonly AzureServiceBusClient azureServiceBusClient;
        private readonly ServiceBusReceiver emailReceiver;
        private readonly ServiceBusReceiver smsReceiver;
        private readonly ServiceBusReceiver templateReceiver;
        private readonly ServiceBusSender templateResponseSender;
        private readonly IEmailSenderService emailSenderService;
        private readonly ISmsSenderService smsSenderService;
        private readonly ITemplateRunnerService templateRunnerService;

        public ServiceBusClient(
            AzureServiceBusSettings azureServiceBusSettings,
            IEmailSenderService emailSenderService,
            ISmsSenderService smsSenderService,
            ITemplateRunnerService templateRunnerService)
        {
            azureServiceBusClient = new AzureServiceBusClient(azureServiceBusSettings.ConnectionString);
            emailReceiver = azureServiceBusClient.CreateReceiver(azureServiceBusSettings.EmailQueueName);
            smsReceiver = azureServiceBusClient.CreateReceiver(azureServiceBusSettings.SmsQueueName);
            templateReceiver = azureServiceBusClient.CreateReceiver(azureServiceBusSettings.TemplateQueueName);
            templateResponseSender = azureServiceBusClient.CreateSender(azureServiceBusSettings.TemplateResponseQueueName);
            this.emailSenderService = emailSenderService;
            this.smsSenderService = smsSenderService;
            this.templateRunnerService = templateRunnerService;
        }

        public async Task StartClient()
        {
            StartEmailProcessing();
            StartSmsProcessing();
            StartTemplateProcessing();
        }

        private async Task StartEmailProcessing()
        {
            var queueResponse = emailReceiver.ReceiveMessagesAsync();
            await foreach (var item in queueResponse)
            {
                try
                {
                    var emailRequest = JsonConvert.DeserializeObject<EmailRequest>(item.Body.ToString());
                    await emailSenderService.SendEmailAsync(emailRequest);
                    await emailReceiver.CompleteMessageAsync(item);
                }
                catch (Exception ex)
                {
                    await emailReceiver.DeadLetterMessageAsync(item);
                }
            }
        }

        private async Task StartSmsProcessing()
        {
            var queueResponse = smsReceiver.ReceiveMessagesAsync();
            await foreach (var item in queueResponse)
            {
                try
                {
                    var smsRequest = JsonConvert.DeserializeObject<SmsRequest>(item.Body.ToString());
                    await smsSenderService.SendSmsAsync(smsRequest);
                    await smsReceiver.CompleteMessageAsync(item);
                }
                catch (Exception ex)
                {
                    await smsReceiver.DeadLetterMessageAsync(item);
                }
            }
        }

        private async Task StartTemplateProcessing()
        {
            var queueResponse = templateReceiver.ReceiveMessagesAsync();
            await foreach(var item in queueResponse)
            {
                try
                {
                    var templateRequest = JsonConvert.DeserializeObject<TemplateRequest>(item.Body.ToString());
                    var subject = item.Subject;
                    var result = await templateRunnerService.RunTemplate(templateRequest.TemplateName, templateRequest.Data);
                    await templateResponseSender.SendMessageAsync(new ServiceBusMessage
                    {
                        Body = BinaryData.FromString(JsonConvert.SerializeObject(result)),
                        Subject = subject
                    });
                    await templateReceiver.CompleteMessageAsync(item);
                } catch(Exception ex)
                {
                    await templateReceiver.DeadLetterMessageAsync(item);
                }
            }
        }
    }
}
