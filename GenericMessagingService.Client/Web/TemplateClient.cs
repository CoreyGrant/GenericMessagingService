using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Sms;
using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Web
{
    internal class TemplateClient : BaseClient, ITemplateClient
    {
        public TemplateClient(WebClientSettings settings, IClassToDictionaryConverter converter) : base(settings, converter)
        {
        }

        public async Task<TemplateResponse> GetTemplate<T>(string templateName, T data) where T : class
        {
            return await GetTemplate(new TemplateRequest { Data = converter.Convert(data), TemplateName = templateName });
        }

        public async Task<TemplateResponse> GetTemplate(string templateName, Dictionary<string, string> data)
        {
            return await GetTemplate(new TemplateRequest { Data = data, TemplateName = templateName });
        }

        private async Task<TemplateResponse> GetTemplate(TemplateRequest templateRequest)
        {
            return await Post<TemplateRequest, TemplateResponse>("/template/", templateRequest);
        }
    }
}
