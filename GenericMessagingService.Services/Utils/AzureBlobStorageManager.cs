using Azure.Storage.Blobs;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Utils
{
    public interface IAzureBlobStorageManager
    {
        Task StoreFile(Stream file, string location, string filename);
        Task StoreFile(string file, string location, string filename);
        Task StoreFile(byte[] file, string location, string filename);
    }

    public class AzureBlobStorageManager : IAzureBlobStorageManager
    {
        private readonly AzureBloblStorageSettings settings;
        private readonly BlobContainerClient client;

        public AzureBlobStorageManager(AzureBloblStorageSettings settings)
        {
            this.settings = settings;
            this.client = new BlobContainerClient(settings.ConnectionString, settings.ContainerName);
        }

        public async Task StoreFile(Stream file, string location, string filename)
        {
            var fullFileName = Path.Join(location, filename);
            var response = await this.client.UploadBlobAsync(fullFileName, file);
        }

        public async Task StoreFile(string file, string location, string filename)
        {
            var fullFileName = Path.Join(location, filename);
            using (var fileStream = new MemoryStream())
            using(var sw = new StreamWriter(fileStream))
            {
                sw.Write(file);
                var response = await this.client.UploadBlobAsync(fullFileName, fileStream);
            }
        }

        public async Task StoreFile(byte[] file, string location, string filename)
        {
            var fullFileName = Path.Join(location, filename);
            using (var fileStream = new MemoryStream())
            using (var sw = new StreamWriter(fileStream))
            {
                sw.Write(file);
                var response = await this.client.UploadBlobAsync(fullFileName, fileStream);
            }
            
        }
    }
}
