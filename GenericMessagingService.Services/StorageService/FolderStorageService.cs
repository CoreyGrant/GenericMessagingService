using GenericMessagingService.Services.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.StorageService
{
    internal class FolderStorageService : IStorageService
    {
        private readonly IFileManager fileManager;

        public FolderStorageService(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public async Task StoreFile(Stream file, string location, string path)
        {
            await fileManager.WriteFileAsync(Path.Join(location, path), file);
        }
    }
}
