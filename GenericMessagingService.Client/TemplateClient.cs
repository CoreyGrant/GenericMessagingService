using GenericMessagingService.Types.Email;
using GenericMessagingService.Types.Shared;
using GenericMessagingService.Types.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client
{
    public interface ITemplateClient : IBaseClient
    {
        Task GetTemplate(TemplateRequest templateRequest);
    }

    internal class TemplateClient : BaseClient, ITemplateClient
    {
        public TemplateClient(ClientSettings settings) : base(settings)
        {
        }

        public async Task GetTemplate(TemplateRequest templateRequest)
        {
            await Post<TemplateRequest, TemplateResponse>("/template/", templateRequest);
        }
    }
}
