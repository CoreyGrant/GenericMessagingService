using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Pdf.Services
{
    public interface IPuppeteerService
    {
        Task<byte[]> GetPdfBytes(string template);
        Task<Stream> GetPdfStream(string template);
    }

    internal class PuppeteerService : IPuppeteerService
    {
        private readonly IPuppeteerPool puppeteerPool;

        public PuppeteerService(IPuppeteerPool puppeteerPool)
        {
            this.puppeteerPool = puppeteerPool;
        }

        public async Task<byte[]> GetPdfBytes(string template)
        {
            return await GetPdfOutput(template, (p) => p.PdfDataAsync());
        }

        public async Task<Stream> GetPdfStream(string template)
        {
            return await GetPdfOutput(template, (p) => p.PdfStreamAsync());
        }

        private async Task<T> GetPdfOutput<T>(string template, Func<IPage, Task<T>> getOutput)
        {
            var browser = await puppeteerPool.GetBrowser();
            var puppet = browser.Browser;
            var page = await puppet.NewPageAsync();
            var output = await getOutput(page);
            browser.Dispose();
            return output;
        }
    }
}
