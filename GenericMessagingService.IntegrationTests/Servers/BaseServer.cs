using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Helpers;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Servers
{
    internal abstract class BaseServer
    {
        private readonly string hostUrl;
        protected readonly string apiKey;

        public BaseServer(string hostUrl, string apiKey)
        {
            this.hostUrl = hostUrl;
            this.apiKey = apiKey;
        }

        protected abstract AppSettings AppSettings { get; }
        public GenericMessagingServiceSettings Settings => new GenericMessagingServiceSettings
        {
            ApiKey = apiKey,
            BindingType = BindingType.Web,
            Config = AppSettings,
        };
        public WebClientSettings WebClientSettings => new WebClientSettings
        {
            ApiKey = apiKey,
            BaseUrl = hostUrl
        };

        public Server Get()
        {
            return ServerHelper.StartServer(Settings, hostUrl);
        }
    }
}
