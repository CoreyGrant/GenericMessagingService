using GenericMessagingService.Services.Utils;
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

        public async Task StoreFile(Stream file, string location, string path)
        {
            await manager.StoreFile(file, location, path);
        }
    }
}
