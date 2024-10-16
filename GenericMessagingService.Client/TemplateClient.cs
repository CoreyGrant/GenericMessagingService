using GenericMessagingService.Client.Utils;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Sms;
using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client
{
    public interface ITemplateClient : IBaseClient
    {
        Task GetTemplate(string templateName, Dictionary<string, string> data);
        Task GetTemplate<T>(string templateName, T data) where T : class;

    }

    internal class TemplateClient : BaseClient, ITemplateClient
    {
        public TemplateClient(ClientSettings settings, IClassToDictionaryConverter converter) : base(settings, converter)
        {
        }

        public async Task GetTemplate<T>(string templateName, T data) where T : class
        {
            await GetTemplate(new TemplateRequest { Data = converter.Convert(data), TemplateName = templateName });
        }

        public async Task GetTemplate(string templateName, Dictionary<string, string> data)
        {
            await GetTemplate(new TemplateRequest { Data = data, TemplateName = templateName });
        }

        private async Task GetTemplate(TemplateRequest templateRequest)
        {
            await Post<TemplateRequest, TemplateResponse>("/template/", templateRequest);
        }
    }
}
