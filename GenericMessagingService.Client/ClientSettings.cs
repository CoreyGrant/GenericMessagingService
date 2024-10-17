using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client
{
    public class ClientSettings
    {
        public AzureServiceBusClientSettings AzureServiceBus { get; set; }
        public WebClientSettings Web { get; set; }
    }

    public class AzureServiceBusClientSettings
    {
        public string ConnectionString { get; set; }
        public string EmailQueueName { get; set; }
        public string SmsQueueName { get; set; }
        public string TemplateQueueName { get; set; }
        public string TemplateResponseQueueName { get; set; }
    }

    public class WebClientSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
}
