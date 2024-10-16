using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Utils
{
    public interface IFileManager
    {
        Task<string> GetFileAsync(string path);
        Task WriteFileAsync(string path, string contents);
        Stream OpenWrite(string path);
    }

    internal class FileManager : IFileManager
    {
        public async Task<string> GetFileAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteFileAsync(string path, string contents)
        {
            await File.WriteAllTextAsync(path, contents);
        }

        public Stream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }
    }
}
