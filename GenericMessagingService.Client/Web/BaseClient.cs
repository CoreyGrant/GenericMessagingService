using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Types.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Web
{
    internal abstract class BaseClient : IBaseClient
    {
        private readonly WebClientSettings settings;
        protected readonly IClassToDictionaryConverter converter;

        public BaseClient(WebClientSettings settings, IClassToDictionaryConverter converter)
        {
            this.settings = settings;
            this.converter = converter;
        }

        protected async Task<T> Get<T>(string url) where T : class
        {
            var client = GetHttpClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {

            }
            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString)!;
            if (apiResponse.Success)
            {
                return apiResponse.Data!;
            }
            throw new Exception(apiResponse.Error);
        }

        protected async Task Post<T>(string url, T data) where T : class
        {
            var client = GetHttpClient();
            var response = await client.PostAsJsonAsync(url, data);
            if (!response.IsSuccessStatusCode)
            {

            }
        }

        protected async Task<V> Post<T, V>(string url, T data) where T : class where V : class
        {
            var client = GetHttpClient();
            var response = await client.PostAsJsonAsync(url, data);
            if (!response.IsSuccessStatusCode)
            {

            }
            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<V>>(jsonString)!;
            if (apiResponse.Success)
            {
                return apiResponse.Data!;
            }
            throw new Exception(apiResponse.Error);
        }

        protected HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(settings.BaseUrl);
            return client;
        }
    }
}
