using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Helpers;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Servers
{
    internal abstract class BaseServer
    {
        public string hostUrl;
        public string apiKey;

        protected abstract AppSettings AppSettings { get; }
        protected AppSettings GetAppSettings(string filename)
        {
            var basePath = AppContext.BaseDirectory;
            var filePath = Path.Join(basePath, "ServerConfig", filename);
            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(filePath));
        }
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
