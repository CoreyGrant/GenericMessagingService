using GenericMessagingService.Types.Attributes;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        Task WriteFileAsync(string path, MemoryStream contents);
        Stream OpenWrite(string path);
        string[] GetFiles(string folder);
        string[] GetFolders(string folder);
        string[] WalkDirectory(string folder);
        char PathSeperator { get; }
    }

    [InjectTransient]
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

        public string[] GetFiles(string folder)
        {
            return Directory.GetFiles(folder);
        }

        public string[] GetFolders(string folder)
        {
            return Directory.GetDirectories(folder);
        }

        public string[] WalkDirectory(string folder)
        {
            var directories = GetFolders(folder);
            var files = GetFiles(folder);
            return files.Concat(directories.SelectMany(WalkDirectory)).ToArray();
        }

        public async Task WriteFileAsync(string path, MemoryStream contents)
        {
            using(var br = new BinaryReader(contents))
            {
                var bytes = contents.ToArray();
                File.WriteAllBytes(path, bytes);
            }
        }

        public char PathSeperator => Path.DirectorySeparatorChar;
    }
}
