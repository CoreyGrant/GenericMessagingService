using GenericMessagingService.Types.Config;

namespace GenericMessagingService.WebApi
{
    public class GenericMessagingServiceSettings
    {
        public string? ConfigPath { get; set; }
        public AppSettings Config { get; set; }
        public BindingType BindingType { get; set; }
        public AzureServiceBusSettings AzureServiceBus { get; set; }
        public string ApiKey { get; set; }
    }

    public class AzureServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string EmailQueueName { get; set; }
        public string SmsQueueName { get; set; }
        public string TemplateQueueName { get; set; }
        public string TemplateResponseQueueName { get; set; }
    }

    [Flags]
    public enum BindingType
    {
        None = 0,
        Web = 1,
        AzureServiceBus = 2,
    } 
}
