using GenericMessagingService.Services.Pdf.Services;
using GenericMessagingService.Services.StorageService;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Attributes;
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
        Task<byte[]> GetPdf(string templateName, IDictionary<string, string> data, string filename);
    }

    [InjectScoped(ServiceType.Pdf)]
    internal class PdfGenerationService : IPdfGenerationService
    {
        private readonly PdfSettings pdfSettings;
        private readonly IPuppeteerService puppeteerService;
        private readonly ITemplateRunnerService templateRunnerService;
        private readonly IAzureBlobStorageServiceFactory azureBlobStorageServiceFactory;
        private readonly IFileManager fileManager;
        private readonly IStorageService storageService;
        private readonly string folderPath;

        public PdfGenerationService(
            PdfSettings pdfSettings,
            IPuppeteerService puppeteerService,
            ITemplateRunnerService templateRunnerService,
            IAzureBlobStorageServiceFactory azureBlobStorageServiceFactory,
            IFileManager fileManager,
            ITemplateStrategyService templateStrategyService) 
        {
            templateStrategyService.TemplateStrategy = pdfSettings.TemplateStrategy;
            this.pdfSettings = pdfSettings;
            this.puppeteerService = puppeteerService;
            this.templateRunnerService = templateRunnerService;
            this.azureBlobStorageServiceFactory = azureBlobStorageServiceFactory;
            this.fileManager = fileManager;
            if (pdfSettings.AzureBlobStorage != null)
            {
                this.storageService = azureBlobStorageServiceFactory.Create(pdfSettings.AzureBlobStorage);
                this.folderPath = pdfSettings.AzureBlobStorage.FolderPath;
            } else if (pdfSettings.Folder != null)
            {
                this.storageService = new FolderStorageService(fileManager);
                this.folderPath = pdfSettings.Folder.FolderPath;
            }
        }

        public async Task<byte[]> GetPdf(string templateName, IDictionary<string, string> data, string filename)
        {
            var result = await templateRunnerService.RunTemplate(templateName, data);
            if(!result.HasValue)
            {
                throw new Exception("Failed to run template for name " + templateName);
            }
            var pdfBytes = await puppeteerService.GetPdfBytes(result.Value.Body);
            if(this.storageService != null)
            {
                await this.storageService.StoreFile(pdfBytes, folderPath, filename);
            }
            return pdfBytes;
        }
    }
}
