using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    public class AzureBlobStorageTemplateLocationService : FileTemplateLocationService, ITemplateLocationService
    {
        private readonly string connectionString;
        private readonly string containerName;
        private readonly IAzureBlobStorageManager azureBlobStorageManager;

        public AzureBlobStorageTemplateLocationService(
            AzureBlobStorageTemplateLocationSettings settings,
            IAzureBlobStorageManager azureBlobStorageManager) : base(settings)
        {
            connectionString = settings.ConnectionString;
            containerName = settings.ContainerName;
            this.azureBlobStorageManager = azureBlobStorageManager;
        }

        public async Task<List<string>> GetTemplateNames()
        {
            var filenames = azureBlobStorageManager.GetFilenames();
            return GetTemplateNames(filenames);
        }

        public async Task<(string?, string?)> LocateTemplateAsync(string templateName)
        {
            if(TryMatchName(templateName, out var path))
            {
                try
                {
                    if (!azureBlobStorageManager.FileExists(path)) { return (null, null); }
                    return (await azureBlobStorageManager.GetFileAsync(path), null);
                }
                catch
                {
                    return (null, null);
                }
            }
            return (null, null);
        }
    }
}
