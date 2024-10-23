using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Interfaces
{
    public interface IPdfClient : IBaseClient
    {
        Task<byte[]> GetPdf(string templateName, Dictionary<string, string> data, string filename);
        Task<byte[]> GetPdf<T>(string templateName, T data, string filename) where T : class;
    }
}
