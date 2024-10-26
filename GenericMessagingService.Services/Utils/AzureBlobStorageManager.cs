using Azure.Storage.Blobs;
using GenericMessagingService.Types.Attributes;
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
        bool FileExists(string filename);
        Task<string> GetFileAsync(string filename);
        List<string> GetFilenames();
    }

    public class AzureBlobStorageManager : IAzureBlobStorageManager
    {
        private readonly AzureBlobStorageSettings settings;
        private readonly BlobContainerClient client;

        public AzureBlobStorageManager(AzureBlobStorageSettings settings)
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

        public bool FileExists(string filename)
        {
            var matchingBlobs = this.client.FindBlobsByTags($"Name = '{filename}'");
            return matchingBlobs.Any();
        }

        public async Task<string> GetFileAsync(string filename)
        {
            var azureResponse = await this.client.GetBlobClient(filename).DownloadContentAsync();
            if(azureResponse.Value != null)
            {
                return azureResponse.Value.Content.ToString();
            }
            return null;
        }

        public List<string> GetFilenames()
        {
            var blobs = this.client.GetBlobs().Select(x => x.Name);
            return blobs.ToList();
        }
    }

    public interface IAzureBlobStorageManagerFactory
    {
        IAzureBlobStorageManager Create(AzureBlobStorageSettings settings);
    }

    [InjectScoped]
    public class AzureBlobStorageManagerFactory : IAzureBlobStorageManagerFactory
    {
        public IAzureBlobStorageManager Create(AzureBlobStorageSettings settings)
        {
            return new AzureBlobStorageManager(settings);
        }
    }
}
