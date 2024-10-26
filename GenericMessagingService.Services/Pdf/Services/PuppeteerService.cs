using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
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

    [InjectScoped(ServiceType.Pdf)]
    internal class PuppeteerService : IPuppeteerService
    {
        private readonly IPuppeteerPool puppeteerPool;
        private readonly PdfSettings settings;

        public PuppeteerService(IPuppeteerPool puppeteerPool, PdfSettings settings)
        {
            this.puppeteerPool = puppeteerPool;
            this.settings = settings;
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
            var browser = await puppeteerPool.GetBrowser(settings.ChromeExePath);
            var puppet = browser.Browser;
            var page = await puppet.NewPageAsync();
            await page.SetContentAsync(template);
            var output = await getOutput(page);
            browser.Dispose();
            return output;
        }
    }
}
