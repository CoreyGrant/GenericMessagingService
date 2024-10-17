using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.AzureServiceBus
{
    public class BaseClient : IBaseClient
    {
        protected readonly IServiceBusClient serviceBusClient;
        protected readonly IClassToDictionaryConverter converter;

        public BaseClient(IServiceBusClient serviceBusClient, IClassToDictionaryConverter converter)
        {
            this.serviceBusClient = serviceBusClient;
            this.converter = converter;
        }
    }
}
