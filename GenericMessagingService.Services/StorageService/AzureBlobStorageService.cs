using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.StorageService
{
    internal class AzureBlobStorageService : IStorageService
    {
        private readonly IAzureBlobStorageManager manager;

        public AzureBlobStorageService(IAzureBlobStorageManager manager) 
        {
            this.manager = manager;
        }

        public async Task StoreFile(MemoryStream file, string location, string path)
        {
            await manager.StoreFile(file, location, path);
        }
    }

    public interface IAzureBlobStorageServiceFactory
    {
        IStorageService Create(AzureBlobStorageSettings settings);
    }

    [InjectTransient]
    public class AzureBlobStorageServiceFactory : IAzureBlobStorageServiceFactory
    {
        private readonly IAzureBlobStorageManagerFactory managerFactory;

        public AzureBlobStorageServiceFactory(IAzureBlobStorageManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        public IStorageService Create(AzureBlobStorageSettings settings)
        {
            return new AzureBlobStorageService(managerFactory.Create(settings));
        }
    }
}
