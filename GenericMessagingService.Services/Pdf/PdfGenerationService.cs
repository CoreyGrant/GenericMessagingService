using GenericMessagingService.Services.Pdf.Services;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Pdf
{
    public interface IPdfGenerationService
    {
        Task<Stream> GetPdf(PdfRequest request);
    }

    internal class PdfGenerationService : IPdfGenerationService
    {
        private readonly PdfSettings pdfSettings;
        private readonly IPuppeteerService puppeteerService;
        private readonly ITemplateRunnerService templateRunnerService;

        public PdfGenerationService(
            PdfSettings pdfSettings,
            IPuppeteerService puppeteerService,
            ITemplateRunnerService templateRunnerService) 
        {
            this.pdfSettings = pdfSettings;
            this.puppeteerService = puppeteerService;
            this.templateRunnerService = templateRunnerService;
            this.azureBlobStorageManager = new AzureBlobStorageManager(pdfSettings.AzureBlobStorage);
        }

        public async Task<Stream> GetPdf(PdfRequest request)
        {
            var result = await templateRunnerService.RunTemplate(request.TemplateName, request.Data);
            if(!result.HasValue)
            {
                throw new Exception("Failed to run template for name " + request.TemplateName);
            }
            var pdfStream = await puppeteerService.GetPdfStream(result.Value.Body);
            if(pdfSettings.AzureBlobStorage != null)
            {
            }
        }
    }
}
