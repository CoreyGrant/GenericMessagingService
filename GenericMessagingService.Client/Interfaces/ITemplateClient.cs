using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Interfaces
{
    public interface ITemplateClient : IBaseClient
    {
        Task<TemplateResponse> GetTemplate(string templateName, Dictionary<string, string> data);
        Task<TemplateResponse> GetTemplate<T>(string templateName, T data) where T : class;
    }
}
