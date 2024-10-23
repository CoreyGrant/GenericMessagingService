using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.StorageService
{
    public interface IStorageService
    {
        public Task StoreFile(string file, string location, string path) 
        {
            using (var stream = new MemoryStream())
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(file);
                return this.StoreFile(stream, location, path);
            }
        }
        public Task StoreFile(MemoryStream file, string location, string path);
        public Task StoreFile(byte[] file, string location, string path)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(file);
                return this.StoreFile(stream, location, path);
            }
        }
    }
}
