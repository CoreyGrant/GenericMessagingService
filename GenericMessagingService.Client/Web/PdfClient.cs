using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Types.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Client.Web
{
    public class PdfClient : BaseClient, IPdfClient
    {
        public PdfClient(WebClientSettings settings, IClassToDictionaryConverter converter) : base(settings, converter)
        {
        }

        public async Task<byte[]> GetPdf(string templateName, Dictionary<string, string> data, string filename)
        {
            var request = new PdfRequest
            {
                TemplateName = templateName,
                Data = data,
                Filename = filename
            };
            var response = await PostWithRawResponse("/api/pdf", request);
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetPdf<T>(string templateName, T data, string filename) where T : class
        {
            return await GetPdf(templateName, converter.Convert(data), filename);
        }
    }
}
